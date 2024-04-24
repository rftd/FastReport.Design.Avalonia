using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;
using ReactiveUI;

namespace FastReport.Designer.ViewModels;

public class AppBootstrapperViewModel : RouterViewModel
{
    #region Fields

    private Report report;
    private readonly Subject<string[]> restartApp;

    #endregion Fields

    #region Constructors

    public AppBootstrapperViewModel(IServiceProvider service) : base(service)
    {
        report = new Report();
        PluginManagerCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var resp = await Dialogs.ShowAsync<PluginManagerDialogViewModel, bool, DialogOptions>(new DialogOptions());
            if(resp == false) return;

            const string mensagem = "Você precisa reinicar a aplicação para que as alterações sejam aplicadas, reiniciar ?";
            resp = await Dialogs.ConfirmAsync("Fast Report Design", mensagem);
            if(resp == false) return;
            
            RestartApp();
        });
        
        restartApp = new Subject<string[]>();
        
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
    
    public IObservable<string[]> OnRestartApp => restartApp.AsObservable();

    public ReactiveCommand<Unit, Unit> PluginManagerCommand { get; }

    #endregion Properties

    #region Methods

    private void RestartApp()
    {
        var args = Environment.GetCommandLineArgs().ToList();
        args.RemoveAt(0);
        
        restartApp.OnNext(args.ToArray());
        restartApp.OnCompleted();
        restartApp.Dispose();
    }

    #endregion Methods
}