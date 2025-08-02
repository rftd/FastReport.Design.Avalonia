using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastReport.Designer.Commom;
using FastReport.Utils;
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

namespace FastReport.Designer.Services;

public sealed class PluginManagerService : IDisposable
{
    #region Fields

    private bool disposedValue;
    private const string NugetFeed = "https://api.nuget.org/v3/index.json";
    private const string FrNugetFeed = "https://nuget.fast-report.com/api/v3/index.json";
    private readonly string[] excludesPlugins = ["FastReport.Data.MsSql"];
    private readonly SourceCacheContext sourceCacheContext;
    private readonly PackageDownloadContext downloadContext;
    private readonly List<FastReportPlugin> availablePlugins;
    private readonly List<FastReportPlugin> installedPlugins;

    #endregion Fields
    
    #region Constructors
    
    public PluginManagerService()
    {
        availablePlugins = [];
        installedPlugins = [];
        sourceCacheContext = new SourceCacheContext();
        downloadContext = new PackageDownloadContext(sourceCacheContext);
        PluginDirectory = Path.Combine(Environment.CurrentDirectory, "Plugins");
        CacheDirectory = Path.Combine(Environment.CurrentDirectory, "Cache");
    }
    
    ~PluginManagerService()
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

    public FastReportPlugin[] AvailablePlugins => availablePlugins.ToArray();
    
    public FastReportPlugin[] InstaledPLugins => installedPlugins.ToArray();
 
    #endregion Properties

    #region Methods
    
    public async Task LoadPluginsAsync()
    {
        availablePlugins.Clear();
        var repository = GetRepository();
        var resource = await repository.GetResourceAsync<PackageSearchResource>();
        var searchFilter = new SearchFilter(includePrerelease: false);

        var metadatas = new List<IPackageSearchMetadata>();

        var page = 0;
        IEnumerable<IPackageSearchMetadata> results;
        do
        {
            var ret = await resource.SearchAsync("FastReport.Data.", searchFilter, 
                skip: 20 * page, take: 20, NullLogger.Instance, CancellationToken.None);

            results = ret.ToArray();
            metadatas.AddRange(results.Where(x => x.Identity.Id.StartsWith("FastReport.Data.") &&
                                                  !excludesPlugins.Contains(x.Identity.Id)));
            page++;
        } while (results.Any());

        var version = NuGetVersion.Parse(Config.Version);
        foreach (var metadata in metadatas)
        {
            var versions = await metadata.GetVersionsAsync();
            var package = versions.SingleOrDefault(x => x.Version == version);
            if(package == null) continue;
            
            availablePlugins.Add(new FastReportPlugin(
                id: metadata.Identity.Id,
                version: version.ToString(),
                name: metadata.Title,
                description: metadata.Description));
        }
        
        LoadInstaledPlugins();
    }
    
    public async Task InstallAsync(FastReportPlugin plugin)
    {
        if(installedPlugins.Any(x => x.Equals(plugin)))
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

        var pluginPath = Path.Combine(PluginDirectory, plugin.Id, plugin.Version);
        
        if (!Directory.Exists(pluginPath))
            Directory.CreateDirectory(pluginPath);

        await SaveFileAsync(Path.Combine(pluginPath, $"{plugin.Id}.dll"), reader.GetStream(item));

        await InstallDependencies(packageId, frameworkVersion, repository);
        
        var plugins = Config.Root.FindItem("Plugins");
        var pluginElement = plugins.Add();
        pluginElement.Name = "Plugin";
        pluginElement.SetProp("Name", Path.Combine(pluginPath, $"{plugin.Id}.dll"));
        pluginElement.SetProp("Id", plugin.Id);
        pluginElement.SetProp("Version", plugin.Version);
        
        installedPlugins.Add(plugin);
    }

    public Task UninstallAsync(FastReportPlugin plugin)
    {
        if(installedPlugins.All(x => !x.Equals(plugin)))
            throw new Exception("Plugin não esta instalado instalado");
        
        var plugins = Config.Root.FindItem("Plugins");

        if (!Directory.Exists(PluginDirectory))
            Directory.CreateDirectory(PluginDirectory);

        var uninstallFile = Path.Combine(PluginDirectory, "plugins.uninstall");
        XmlItem pluginsNode;
        var uninstallDoc = new XmlDocument();
        if (File.Exists(uninstallFile))
        {
            uninstallDoc.Load(uninstallFile);
            pluginsNode = uninstallDoc.Root.FindItem("Plugins");
        }
        else
        {
            uninstallDoc.Root.Name = "Uninstall";
            pluginsNode = uninstallDoc.Root.Add();
            pluginsNode.Name = "Plugins";
        }

        XmlItem? pluginToUninstall = null;
        foreach (var item in plugins.Items)
        {
            var id = item.GetProp("Id");
            if (string.IsNullOrEmpty(id))
                id = Path.GetFileNameWithoutExtension(item.GetProp("Name"));

            if (id != plugin.Id) continue;
            
            pluginToUninstall = item;
            break;
        }

        if (pluginToUninstall == null) throw new Exception("Plugin não encontrado");

        pluginsNode.AddItem(pluginToUninstall);
        plugins.Items.Remove(pluginToUninstall);
        availablePlugins.Add(plugin);
        
        uninstallDoc.Save(uninstallFile);
        return Task.CompletedTask;
    }
    
    private void LoadInstaledPlugins()
    {
        installedPlugins.Clear();
        
        var plugins = Config.Root.FindItem("Plugins");
        foreach (var item in plugins.Items)
        {
            var id = item.GetProp("Id");
            if (string.IsNullOrEmpty(id))
                id = Path.GetFileNameWithoutExtension(item.GetProp("Name"));

            var plugin = availablePlugins.Find(x => x.Id == id);
            if(plugin == null) continue;
            
            var version = item.GetProp("Version");
            if (string.IsNullOrEmpty(id))
                version = plugin.Version;
            
            installedPlugins.Add(new FastReportPlugin(plugin.Id, plugin.Name, plugin.Description, version));
        }
    }
    
    private async Task InstallDependencies(PackageIdentity packageId,
        NuGetFramework frameworkVersion, SourceRepository repository)
    {
        var pluginPath = Path.Combine(PluginDirectory, packageId.Id);
        var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
        await GetPackageDependenciesInfo(packageId, frameworkVersion, repository, availablePackages);
        
        var resolverContext = new PackageResolverContext(
            DependencyBehavior.Lowest, [packageId.Id],
            [], [],
            [], availablePackages, [repository.PackageSource],
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
            PackageReaderBase reader;
            var installedPath = packagePathResolver.GetInstalledPath(packageToInstall);
            
            if (installedPath == null)
            {
                var downloadResult = await DownloadPackage(repository, packageToInstall);
                await PackageExtractor.ExtractPackageAsync(downloadResult.PackageSource, 
                    downloadResult.PackageStream, packagePathResolver, packageExtractionContext, 
                    CancellationToken.None);

                reader = downloadResult.PackageReader;
            }
            else
            {
                reader = new PackageFolderReader(installedPath);
            }
            
            var libItems = reader.GetLibItems().ToArray();
            var nearest = frameworkReducer.GetNearest(frameworkVersion, libItems.Select(x => x.TargetFramework));
            var items = libItems.Where(x => x.TargetFramework.Equals(nearest))
                .SelectMany(x => x.Items).Where(x => x.EndsWith(".dll")).ToArray();
            
            foreach (var item in items)
                await SaveFileAsync(Path.Combine(pluginPath, Path.GetFileName(item)), reader.GetStream(item));
           
            var frameworkItems = reader.GetFrameworkItems().ToArray();
            nearest = frameworkReducer.GetNearest(frameworkVersion, frameworkItems.Select(x => x.TargetFramework));

            items = frameworkItems.Where(x => x.TargetFramework.Equals(nearest))
                .SelectMany(x => x.Items).Where(x => x.EndsWith(".dll")).ToArray();
            
            foreach (var item in items)
                await SaveFileAsync(Path.Combine(pluginPath, Path.GetFileName(item)), reader.GetStream(item));
        }
    }

    private static async Task SaveFileAsync(string targetPath, Stream stream)
    {
        await using var fileStream = File.Open(targetPath, FileMode.OpenOrCreate);
        fileStream.Position = 0;
        await stream.CopyToAsync(fileStream);
        fileStream.SetLength(fileStream.Position);
    }
    
    private async Task<DownloadResourceResult> DownloadPackage(SourceRepository repository, PackageIdentity packageId)
    {
        var resource = await repository.GetResourceAsync<DownloadResource>();
        return await resource.GetDownloadResourceResultAsync(
            packageId, downloadContext, globalPackagesFolder: CacheDirectory, 
            logger: NullLogger.Instance, token: CancellationToken.None);
    }
    
    private async Task GetPackageDependenciesInfo(PackageIdentity package, NuGetFramework framework, 
        SourceRepository repository, ISet<SourcePackageDependencyInfo> availablePackages)
    {
        if (availablePackages.Contains(package)) return;

        var dependencyInfoResource = await repository.GetResourceAsync<DependencyInfoResource>();
        var dependencyInfo = await dependencyInfoResource.ResolvePackage(
            package, framework, sourceCacheContext, NullLogger.Instance, CancellationToken.None);

        if (dependencyInfo == null) return;

        availablePackages.Add(dependencyInfo);
        foreach (var dependency in dependencyInfo.Dependencies)
        {
            await GetPackageDependenciesInfo(
                new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
                framework, repository, availablePackages);
        }
    }

    private SourceRepository GetRepository()
    {
        PackageSource source;
        if (UseFrNugetFeed)
        {
            if(string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                throw new Exception("Username and password are required.");
            
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