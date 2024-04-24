using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;
using DynamicData;
using FastReport.Designer.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FastReport.Designer.ViewModels;

public sealed class PluginManagerDialogViewModel : MvvmDialogViewModel<DialogOptions, bool>
{
    #region Fields

    private readonly FrPluginManager manager;
    private bool hasInstallUninstall;

    #endregion Fields

    #region Constructors

    public PluginManagerDialogViewModel(IServiceProvider service) : base(service)
    {
        manager = Service.GetRequiredService<FrPluginManager>();
        
        var isBusy = this.WhenAnyValue(x => x.IsBusy, x => x == false);
        
        ReloadPluginsCommand = ReactiveCommand.CreateFromTask(LoadPluginsAsync, isBusy);
        CloseDialogCommand = ReactiveCommand.Create(OnClose, isBusy);
        
        var canInstall = this.WhenAnyValue(x => x.IsBusy, x => x.PluginsToInstall.Count, (x, y) => x == false && y > 0);
        var canUninstall = this.WhenAnyValue(x => x.IsBusy, x => x.PluginsToUninstall.Count, (x, y) => x == false && y > 0);
        
        InstallCommand = ReactiveCommand.CreateFromTask(InstallPluginsAsync, canInstall);
        UninstallCommand = ReactiveCommand.CreateFromTask(UninstallPluginsAsync, canUninstall);
    }

    #endregion Constructors

    #region Properties
    
    public ReactiveCommand<Unit, Unit> InstallCommand { get; }
    
    public ReactiveCommand<Unit, Unit> UninstallCommand { get; }
    
    public ReactiveCommand<Unit, Unit> CloseDialogCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ReloadPluginsCommand { get; }

    public ObservableCollection<FrPlugin> AvailablePlugins { get; } = [];
    
    public ObservableCollection<FrPlugin> PluginsToInstall { get; } = [];
    
    public ObservableCollection<FrPlugin> InstaledPLugins { get; } = [];
    
    public ObservableCollection<FrPlugin> PluginsToUninstall { get; } = [];

    #endregion Properties

    #region Methods

    public override async void Initialize(DialogOptions parameter)
    {
        await LoadPluginsAsync();
    }

    private void OnClose()
    {
        SetResult(hasInstallUninstall);
    }
    
    private async Task LoadPluginsAsync()
    {
        IsBusy = true;

        try
        {
            await manager.LoadPluginsAsync();
        
            AvailablePlugins.Clear();
            AvailablePlugins.AddRange(manager.AvailablePlugins.Except(manager.InstaledPLugins));
            InstaledPLugins.AddRange(manager.InstaledPLugins);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task InstallPluginsAsync()
    {
        IsBusy = true;

        try
        {
            foreach (var plugin in PluginsToInstall)
                await manager.InstallAsync(plugin);
        
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
    
    private async Task UninstallPluginsAsync()
    {
        IsBusy = true;

        try
        {
            foreach (var plugin in PluginsToUninstall)
                await manager.UninstallAsync(plugin);
        
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