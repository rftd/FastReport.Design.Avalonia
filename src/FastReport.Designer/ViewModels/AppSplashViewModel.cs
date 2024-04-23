using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Caramelo.MvvmApp.ViewModel;
using DynamicData;
using FastReport.Utils;
using ReactiveUI;
using Splat;

namespace FastReport.Designer.ViewModels;

public class AppSplashViewModel : MvvmSplashViewModel
{
    #region Fields

    private string message;

    #endregion Fields

    #region Constructors

    public AppSplashViewModel(IServiceProvider service) : base(service)
    {
        message = "Iniciando o Designer";
    }

    #endregion Constructors

    #region Properties

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
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FastReport");
        var configFile = Path.Combine(path, "FastReport.Avalonia.config");

        var configDoc = new XmlDocument();
        configDoc.Load(configFile);

        var plugins = configDoc.Root.FindItem("Plugins");
        if (plugins.Items.Count < 1)
            await Task.Delay(3000);
        else
            Message = "Desinstalando plugins";

        var toRemove = new List<XmlItem>();
        foreach (var item in plugins.Items)
        {
            var uninstall = item.GetProp("Uninstall");
            if(string.IsNullOrEmpty(uninstall)) continue;
            
            toRemove.Add(item);
            var id = item.GetProp("Id");
            if (string.IsNullOrEmpty(id))
                id = Path.GetFileNameWithoutExtension(item.GetProp("Name"));

            try
            {
                var pluginPath = Path.Combine(Environment.CurrentDirectory, "Plugins", id);
                ClearFolder(pluginPath);
                Directory.Delete(pluginPath);
            }
            catch (Exception)
            {
                //Ignore
            }
        }
        
        plugins.Items.RemoveMany(toRemove);
        configDoc.Save(configFile);
        configDoc.Dispose();
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
    }

    #endregion Methods
}