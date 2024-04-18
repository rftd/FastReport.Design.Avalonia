using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using ZipArchive = System.IO.Compression.ZipArchive;

namespace FastReport.Designer.Services;

public sealed class FrPluginManager : IDisposable
{
    #region Fields

    private const string NugetFeed = "https://api.nuget.org/v3/index.json";
    private const string FrNugetFeed = "https://nuget.fast-report.com/api/v3/index.json";
    private readonly string[] excludesPlugins = ["FastReport.Data.MsSql"];
    private readonly SourceCacheContext sourceCacheContext;
    private bool disposedValue;
    private List<FrPlugin> installedPlugins;

    #endregion Fields
    
    #region Constructors
    
    public FrPluginManager()
    {
        installedPlugins = new List<FrPlugin>();
        sourceCacheContext = new SourceCacheContext();
        PluginDirectory = Path.Combine(Environment.CurrentDirectory, "Plugins");
        CacheDirectory = Path.Combine(Environment.CurrentDirectory, "Cache");
        LoadPLugins();
    }
    
    ~FrPluginManager()
    {
        Dispose(false);
    }

    #endregion Constructors 
 
    #region Properties

    public string PluginDirectory { get; set; }
    
    public string CacheDirectory { get; set; }

    public bool UseFrNugetFeed { get; set; } = false;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public FrPlugin[] InstaledPLugins => installedPlugins.ToArray();
 
    #endregion Properties

    #region Methods

    private async void LoadPLugins()
    {
        installedPlugins.Clear();
        var repository = GetRepository();
        var resource = await repository.GetResourceAsync<PackageMetadataResource>();
        
        var plugins = Utils.Config.Root.FindItem("Plugins");
        foreach (var item in plugins.Items)
        {
            var name = Path.GetFileNameWithoutExtension(item.GetProp("Name"));
            var id = item.GetProp("Id");
            if (string.IsNullOrEmpty(id))
                id = name;
            
            var versao = item.GetProp("Version");
            if (string.IsNullOrEmpty(versao))
                versao = Utils.Config.Version;
            
            var packageIdentity = new PackageIdentity(id, NuGetVersion.Parse(versao));
            var metadata = await resource.GetMetadataAsync(packageIdentity, sourceCacheContext, 
                NullLogger.Instance, CancellationToken.None);
            
            
            installedPlugins.Add(new FrPlugin(id: id, name: name, description: metadata.Description, version: versao));
        }
    }
    
    public async Task<FrPlugin[]> GetPluginsAsync()
    {
        var repository = GetRepository();
        var resource = await repository.GetResourceAsync<PackageSearchResource>();
        var searchFilter = new SearchFilter(includePrerelease: false);

        var metadatas = new List<IPackageSearchMetadata>();

        var page = 0;
        IEnumerable<IPackageSearchMetadata> results;
        do
        {
            var ret = await resource.SearchAsync(
                "FastReport.Data.",
                searchFilter,
                skip: 20 * page,
                take: 20,
                NullLogger.Instance,
                CancellationToken.None);

            results = ret.ToArray();
            metadatas.AddRange(results.Where(x => x.Identity.Id.StartsWith("FastReport.Data.") &&
                                                  !excludesPlugins.Contains(x.Identity.Id)));
            page++;
        } while (results.Any());

        var version = NuGetVersion.Parse(Utils.Config.Version);
        var plugins = new List<FrPlugin>();
        foreach (var metadata in metadatas)
        {
            var versions = await metadata.GetVersionsAsync();
            var package = versions.SingleOrDefault(x => x.Version == version);
            if(package == null) continue;
            
            plugins.Add(new FrPlugin(
                id: metadata.Identity.Id,
                version: Utils.Config.Version,
                name: metadata.Title,
                description: metadata.Description));
        }
        
        return plugins.ToArray();
    }
    
    public async Task InstallAsync(FrPlugin plugin)
    {
        if(installedPlugins.Any(x => x.Id == plugin.Id))
            throw new Exception("Plugin já instalado");
        
        var repository = GetRepository();

        var packageId = new PackageIdentity(plugin.Id, new NuGetVersion(plugin.Version));
        var frameworkVersion = NuGetFramework.ParseFolder("net6.0");

        using var downloadResourceResult = await DownloadPackage(repository, packageId);
        
        if (downloadResourceResult.Status != DownloadResourceResultStatus.Available)
            throw new Exception($"Download of NuGet package failed. DownloadResult Status: {downloadResourceResult.Status}");

        var item = $"build/net6.0/FastReport.Avalonia/{plugin.Id}.dll";
        
        var reader = downloadResourceResult.PackageReader;
        var lib = reader.GetFiles().SingleOrDefault(x => x == item);
        if (lib == null) throw new ApplicationException("Plugin Não encontrado");

        if (!Directory.Exists(PluginDirectory))
            Directory.CreateDirectory(PluginDirectory);
 
        await reader.CopyFilesAsync(PluginDirectory, [item], SaveFile, 
            NullLogger.Instance, CancellationToken.None);

        await InstallDependencies(packageId, frameworkVersion, repository);
        
        var plugins = Utils.Config.Root.FindItem("Plugins");
        var pluginElement = plugins.Add();
        pluginElement.Name = "Plugin";
        pluginElement.SetProp("Name", Path.Combine(PluginDirectory, $"{plugin.Id}.dll"));
        pluginElement.SetProp("Id", plugin.Id);
        pluginElement.SetProp("Version", plugin.Version);
        
        installedPlugins.Add(plugin);
        ClearFolder(PluginDirectory);
    }

    private async Task InstallDependencies(PackageIdentity packageId,
        NuGetFramework frameworkVersion, SourceRepository repository)
    {
        var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
        await GetPackageDependencies(packageId, frameworkVersion, repository, availablePackages);
        
        var resolverContext = new PackageResolverContext(
            DependencyBehavior.Lowest, [packageId.Id],
            Enumerable.Empty<string>(), Enumerable.Empty<PackageReference>(),
            Enumerable.Empty<PackageIdentity>(), availablePackages, [repository.PackageSource],
            NullLogger.Instance);
        
        var frameworkReducer = new FrameworkReducer();
        
        var resolver = new PackageResolver();
        var packagesToInstall = resolver.Resolve(resolverContext, CancellationToken.None)
            .Select(p => availablePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)));
        
        var cliCtx = ClientPolicyContext.GetClientPolicy(NullSettings.Instance, NullLogger.Instance);
        var packagePathResolver = new PackagePathResolver(CacheDirectory);
        var packageExtractionContext = new PackageExtractionContext(PackageSaveMode.Defaultv3,
            XmlDocFileSaveMode.None, cliCtx, NullLogger.Instance);
        
        foreach (var packageToInstall in packagesToInstall)
        {
            PackageReaderBase packageReader;
            var installedPath = packagePathResolver.GetInstalledPath(packageToInstall);
            
            if (installedPath == null)
            {
                var downloadResource = await packageToInstall.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);
                var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                    packageToInstall, new PackageDownloadContext(sourceCacheContext),
                    CacheDirectory, NullLogger.Instance, CancellationToken.None);

                await PackageExtractor.ExtractPackageAsync(
                    downloadResult.PackageSource,
                    downloadResult.PackageStream,
                    packagePathResolver,
                    packageExtractionContext,
                    CancellationToken.None);

                packageReader = downloadResult.PackageReader;
            }
            else
            {
                packageReader = new PackageFolderReader(installedPath);
            }
            
            var libItems = packageReader.GetLibItems().ToArray();
            var nearest = frameworkReducer.GetNearest(frameworkVersion, libItems.Select(x => x.TargetFramework));
            var items = libItems.Where(x => x.TargetFramework.Equals(nearest))
                .SelectMany(x => x.Items).Where(x => x.EndsWith(".dll")).ToArray();

            await packageReader.CopyFilesAsync(AppDomain.CurrentDomain.BaseDirectory, items, SaveFile, 
                NullLogger.Instance, CancellationToken.None);
            
            var frameworkItems = packageReader.GetFrameworkItems().ToArray();
            nearest = frameworkReducer.GetNearest(frameworkVersion, frameworkItems.Select(x => x.TargetFramework));

            items = frameworkItems.Where(x => x.TargetFramework.Equals(nearest))
                .SelectMany(x => x.Items).Where(x => x.EndsWith(".dll")).ToArray();
            
            await packageReader.CopyFilesAsync(AppDomain.CurrentDomain.BaseDirectory, items, SaveFile, 
                NullLogger.Instance, CancellationToken.None);
        }
    }

    private string SaveFile(string sourceFile, string targetPath, Stream stream)
    {
        var filePath = Path.Combine(PluginDirectory, Path.GetFileName(sourceFile));

        if (File.Exists(filePath))
            File.Replace(filePath, sourceFile, null);
        else
            stream.CopyToFile(filePath);

        return filePath;
    }
    
    private async Task<DownloadResourceResult> DownloadPackage(SourceRepository repository, PackageIdentity packageId)
    {
        var resource = await repository.GetResourceAsync<DownloadResource>();
        return await resource.GetDownloadResourceResultAsync(
            packageId, new PackageDownloadContext(sourceCacheContext),
            globalPackagesFolder: CacheDirectory, logger: NullLogger.Instance,
            token: CancellationToken.None);
    }
    
    private async Task GetPackageDependencies(PackageIdentity package,
        NuGetFramework framework, SourceRepository repository,
        ISet<SourcePackageDependencyInfo> availablePackages)
    {
        if (availablePackages.Contains(package)) return;


        var dependencyInfoResource = await repository.GetResourceAsync<DependencyInfoResource>();
        var dependencyInfo = await dependencyInfoResource.ResolvePackage(
            package, framework, sourceCacheContext, NullLogger.Instance, CancellationToken.None);

        if (dependencyInfo == null) return;

        availablePackages.Add(dependencyInfo);
        foreach (var dependency in dependencyInfo.Dependencies)
        {
            await GetPackageDependencies(
                new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
                framework, repository, availablePackages);
        }
    }

    private SourceRepository GetRepository()
    {
        PackageSource source;
        if (UseFrNugetFeed)
        {
            source = new PackageSource(FrNugetFeed)
            {
                Credentials = new PackageSourceCredential(
                    source: FrNugetFeed,
                    username: Username,
                    passwordText: Password,
                    isPasswordClearText: true,
                    validAuthenticationTypesText: null)
            };
        }
        else
        {
            source = new PackageSource(NugetFeed);
        }

        return Repository.Factory.GetCoreV3(source);
    }

    private void ClearFolder(string folderName)
    {
        var dir = new DirectoryInfo(folderName);
        foreach (var di in dir.GetDirectories())
        {
            ClearFolder(di.FullName);
            di.Delete();
        }
    }
    
    private void Dispose(bool disposing)
    {
        // check if already disposed 
        if (disposedValue) return;
        
        if (disposing)
            sourceCacheContext.Dispose();
        
        // set the bool value to true 
        disposedValue = true;
    } 

    public void Dispose() 
    { 
        // Invoke the above virtual 
        // dispose(bool disposing) method 
        Dispose(true); 
  
        // Notify the garbage collector 
        // about the cleaning event 
        GC.SuppressFinalize(this); 
    }
    
    #endregion Methods
}