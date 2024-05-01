using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caramelo.MvvmApp.ViewModel;
using FastReport.Designer.Services;
using FastReport.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace FastReport.Designer.ViewModels;

public class AppSplashViewModel : MvvmSplashViewModel
{
    #region Fields

    private string message;
    private readonly PluginManagerService pluginManager;

    #endregion Fields

    #region Constructors

    public AppSplashViewModel(IServiceProvider service) : base(service)
    {
        message = "Iniciando o Designer";
        pluginManager = service.GetRequiredService<PluginManagerService>();
        Log = service.GetRequiredService<ILogger<AppSplashViewModel>>();
        this.WhenAnyValue(x => x.Message).Subscribe(msg => Log.LogInformation(msg));
    }

    #endregion Constructors

    #region Properties

    public ILogger<AppSplashViewModel> Log { get; }
    
    public string Message
    {
        get => message;
        set => this.RaiseAndSetIfChanged(ref message, value);
    }

    #endregion Properties

    #region Methods

    public override void ViewAppeared()
    {
        VerifyPlugins();
    }

    private async void VerifyPlugins()
    {
        var uninstallFile = Path.Combine(pluginManager.PluginDirectory, "plugins.uninstall");
        if (File.Exists(uninstallFile))
        {
            Message = "Removendo Plugins";
            
            var uninstallDoc = new XmlDocument();
            uninstallDoc.Load(uninstallFile);
            var plugins = uninstallDoc.Root.FindItem("Plugins");
            foreach (var pluginPath in plugins.Items
                         .Select(item => Path.GetDirectoryName(item.GetProp("Name")))
                         .Where(Path.Exists).Cast<string>())
            {
                try
                {
                    ClearFolder(pluginPath);
                    Directory.Delete(pluginPath);
                }
                catch (Exception)
                {
                    //
                }
            }

            File.Delete(uninstallFile);
            Message = "Plugins Removidos";
        }
        else
        {
            await Task.Delay(3000);
        }
       
        CloseSplash();
    }
    
    private static void ClearFolder(string folderName)
    {
        var dir = new DirectoryInfo(folderName);
        foreach (var di in dir.GetDirectories())
        {
            ClearFolder(di.FullName);
            foreach (var file in di.GetFiles())
                File.Delete(file.FullName);
            
            di.Delete();
        }
        
        foreach (var file in dir.GetFiles())
            File.Delete(file.FullName);
    }

    #endregion Methods
}