using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;
using DynamicData;
using FastReport.Designer.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace FastReport.Designer.ViewModels;

public sealed class PluginManagerDialogViewModel : MvvmDialogViewModel<DialogOptions, Unit>
{
    #region Fields

    private readonly FrPluginManager manager;

    #endregion Fields

    #region Constructors

    public PluginManagerDialogViewModel(IServiceProvider service) : base(service)
    {
        manager = Service.GetRequiredService<FrPluginManager>();
        
        var isBusy = this.WhenAnyValue(x => x.IsBusy, x => x == false);
        
        ReloadPluginsCommand = ReactiveCommand.CreateFromTask(LoadPluginsAsync, isBusy);
        CloseDialogCommand = ReactiveCommand.Create(() => SetResult(Unit.Default), isBusy);
        
        var canInstall = this.WhenAnyValue(x => x.IsBusy, x => x.PluginsToInstall, (x, y) => x == false && y.Any());
        var canUninstall = this.WhenAnyValue(x => x.IsBusy, x => x.PluginsToUninstall, (x, y) => x == false && y.Any());
        
        InstallCommand = ReactiveCommand.CreateFromTask(InstallPluginsAsync, canInstall);
        UninstallCommand = ReactiveCommand.CreateFromTask(UninstallPluginsAsync, canUninstall);
    }

    #endregion Constructors

    #region Properties
    
    public ReactiveCommand<Unit, Unit> InstallCommand { get; }
    
    public ReactiveCommand<Unit, Unit> UninstallCommand { get; }
    
    public ReactiveCommand<Unit, Unit> CloseDialogCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ReloadPluginsCommand { get; }

    public ObservableCollection<FrPlugin> AvailablePlugins { get; } = new();
    
    public ObservableCollection<FrPlugin> PluginsToInstall { get; } = new();
    
    public ObservableCollection<FrPlugin> InstaledPLugins { get; } = new();
    
    public ObservableCollection<FrPlugin> PluginsToUninstall { get; } = new();

    #endregion Properties

    #region Methods

    public override async void Initialize(DialogOptions parameter)
    {
        await LoadPluginsAsync();
    }

    private async Task LoadPluginsAsync()
    {
        await manager.LoadPluginsAsync();
        
        AvailablePlugins.Clear();
        ExtensionMethods.AddRange(AvailablePlugins, manager.AvailablePlugins.Except(manager.InstaledPLugins));
        
        ExtensionMethods.AddRange(InstaledPLugins, manager.InstaledPLugins);
    }

    private async Task InstallPluginsAsync()
    {
        IsBusy = true;

        try
        {
            foreach (var plugin in PluginsToInstall)
                await manager.InstallAsync(plugin);
        
            AvailablePlugins.RemoveMany(PluginsToInstall);
            PluginsToInstall.Clear();
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private async Task UninstallPluginsAsync()
    {
        // foreach (var plugin in PluginsToUninstall)
        //     await manager.InstallAsync(plugin);
    }

    #endregion Methods
}