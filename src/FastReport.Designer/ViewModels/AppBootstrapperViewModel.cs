using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;
using FastReport.Designer.Commom;
using FastReport.Utils;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.SourceGenerators;

namespace FastReport.Designer.ViewModels;

public partial class AppBootstrapperViewModel : RouterViewModel
{
    #region Fields

    [Reactive]
    private Report report;
    
    private readonly Subject<WelcomeResult> welcomeResult;

    #endregion Fields

    #region Constructors

    public AppBootstrapperViewModel(IServiceProvider service) : base(service)
    {
        report = new Report();
        MenuHelper = Service.GetRequiredService<FrDesignerMenuHelper>();
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
    
    public FrDesignerMenuHelper MenuHelper { get; }
    
    public IObservable<WelcomeResult> OnWelcomeDone => welcomeResult.AsObservable();

    #endregion Properties

    #region Methods

    [ReactiveCommand]
    private async Task PluginManager()
    {
        var resp = await Dialogs.ShowAsync<PluginManagerDialogViewModel, bool, DialogOptions>(new DialogOptions());
        if(resp == false) return;

        const string mensagem = "A aplicação precisa ser reiniciada para aplicar as alterações.";
        await Dialogs.WarnAsync(mensagem);
        RestartApp();
    }

    [ReactiveCommand]
    private void FinishApp() => base.FinishApp();

    public override async void ViewAppeared()
    {
        if(!Config.WelcomeEnabled) return;
        
        var ret = await Dialogs.ShowAsync<WelcomeDialogViewModel, WelcomeResult, DialogOptions>(new DialogOptions());
        
        welcomeResult.OnNext(ret);
    }

    #endregion Methods
}