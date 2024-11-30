using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;
using DynamicData;
using FastReport.Designer.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace FastReport.Designer.ViewModels;

public partial class PluginManagerDialogViewModel : MvvmDialogViewModel<DialogOptions, bool>
{
    #region Fields

    private readonly PluginManagerService managerService;
    private bool hasInstallUninstall;
    private IObservable<bool> canInstall;
    private IObservable<bool> canUninstall;
    private IObservable<bool> isBusy;

    #endregion Fields

    #region Constructors

    public PluginManagerDialogViewModel(IServiceProvider service) : base(service)
    {
        managerService = Service.GetRequiredService<PluginManagerService>();
        
        isBusy = this.WhenAnyValue(x => x.IsBusy, x => x == false);
        canInstall = this.WhenAnyValue(x => x.IsBusy, x => x.PluginsToInstall.Count, (x, y) => x == false && y > 0);
        canUninstall = this.WhenAnyValue(x => x.IsBusy, x => x.PluginsToUninstall.Count, (x, y) => x == false && y > 0);
    }

    #endregion Constructors

    #region Properties

    [Reactive]
    private ObservableCollection<FrPlugin> availablePlugins = [];

    [Reactive]
    private ObservableCollection<FrPlugin> pluginsToInstall = [];

    [Reactive]
    private ObservableCollection<FrPlugin> instaledPLugins = [];

    [Reactive]
    private ObservableCollection<FrPlugin> pluginsToUninstall = [];

    #endregion Properties

    #region Methods

    public override async void Initialize(DialogOptions parameter)
    {
        await ReloadPlugins();
    }

    [ReactiveCommand]
    private void CloseDialog()
    {
        SetResult(hasInstallUninstall);
    }
    
    [ReactiveCommand(CanExecute = nameof(isBusy))]
    private async Task ReloadPlugins()
    {
        IsBusy = true;

        try
        {
            await managerService.LoadPluginsAsync();
        
            AvailablePlugins.Clear();
            AvailablePlugins.AddRange(managerService.AvailablePlugins.Except(managerService.InstaledPLugins));
            InstaledPLugins.AddRange(managerService.InstaledPLugins);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ReactiveCommand(CanExecute = nameof(canInstall))]
    private async Task Install()
    {
        IsBusy = true;

        try
        {
            foreach (var plugin in PluginsToInstall)
                await managerService.InstallAsync(plugin);
        
            InstaledPLugins.AddRange(PluginsToInstall);
            AvailablePlugins.RemoveMany(PluginsToInstall);
            PluginsToInstall.Clear();
            hasInstallUninstall = true;
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    [ReactiveCommand(CanExecute = nameof(canUninstall))]
    private async Task Uninstall()
    {
        IsBusy = true;

        try
        {
            foreach (var plugin in PluginsToUninstall)
                await managerService.UninstallAsync(plugin);
        
            InstaledPLugins.RemoveMany(PluginsToUninstall);
            AvailablePlugins.AddRange(PluginsToUninstall);
            PluginsToUninstall.Clear();
            hasInstallUninstall = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion Methods
}