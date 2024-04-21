using System;
using System.Reactive;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;
using ReactiveUI;

namespace FastReport.Designer.ViewModels;

public class AppBootstrapperViewModel : RouterViewModel
{
    #region Fields

    private Report report;

    #endregion Fields

    #region Constructors

    public AppBootstrapperViewModel(IServiceProvider service) : base(service)
    {
        report = new Report();
        PluginManagerCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Dialogs.ShowAsync<PluginManagerDialogViewModel, Unit, DialogOptions>(new DialogOptions());
        });
        
        var args = Environment.GetCommandLineArgs();
        if(args.Length < 2) return;
        
        try
        {
            report.Load(args[1]);
        }
        catch (Exception)
        {
            Dialogs.ErroAsync("Erro ao carregar o report");
        }
    }

    #endregion Constructors

    #region Properties

    public Report Report
    {
        get => report;
        set => this.RaiseAndSetIfChanged(ref report, value);
    }

    public ReactiveCommand<Unit, Unit> PluginManagerCommand { get; }

    #endregion Properties
}