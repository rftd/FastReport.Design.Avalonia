using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;
using FastReport.Designer.Commom;
using FastReport.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace FastReport.Designer.ViewModels;

public class AppBootstrapperViewModel : RouterViewModel
{
    #region Fields

    private Report report;
    private readonly Subject<string[]> restartApp;
    private readonly Subject<WelcomeResult> welcomeResult;

    #endregion Fields

    #region Constructors

    public AppBootstrapperViewModel(IServiceProvider service) : base(service)
    {
        report = new Report();
        MenuHelper = Service.GetRequiredService<FrDesignerMenuHelper>();
        PluginManagerCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var resp = await Dialogs.ShowAsync<PluginManagerDialogViewModel, bool, DialogOptions>(new DialogOptions());
            if(resp == false) return;

            const string mensagem = "A aplicação precisa ser reiniciada para aplicar as alterações.";
            await Dialogs.WarnAsync(mensagem);
            RestartApp();
        });

        PluginManagerCommand.ThrownExceptions.Subscribe(ex =>
        {
            Log.LogError(ex, "Erro ao abrir a tela de gerenciamento de plugin");
        });
        
        restartApp = new Subject<string[]>();
        welcomeResult = new Subject<WelcomeResult>();
        
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

    public FrDesignerMenuHelper MenuHelper { get; }
    
    public IObservable<string[]> OnRestartApp => restartApp.AsObservable();
    
    public IObservable<WelcomeResult> OnWelcomeDone => welcomeResult.AsObservable();

    public ReactiveCommand<Unit, Unit> PluginManagerCommand { get; }

    #endregion Properties

    #region Methods

    public override async void ViewAppeared()
    {
        if(!Config.WelcomeEnabled) return;
        
        var ret = await Dialogs.ShowAsync<WelcomeDialogViewModel, WelcomeResult, DialogOptions>(new DialogOptions());
        
        welcomeResult.OnNext(ret);
    }

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