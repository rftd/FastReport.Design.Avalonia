using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;
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
        ReloadPluginsCommand = ReactiveCommand.CreateFromTask(LoadPluginsAsync);
    }

    #endregion Constructors

    #region Properties
    
    public ReactiveCommand<Unit, Unit> ReloadPluginsCommand { get; }

    public ObservableCollection<FrPlugin> AvailablePlugins { get; } = new();
    
    public ObservableCollection<FrPlugin> InstaledPLugins { get; } = new();

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
        AvailablePlugins.AddRange(manager.AvailablePlugins.Except(manager.InstaledPLugins));
        
        InstaledPLugins.AddRange(manager.InstaledPLugins);
    }

    #endregion Methods
}