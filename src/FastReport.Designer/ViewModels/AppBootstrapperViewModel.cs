using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using FastReport.Designer.Commom;
using FastReport.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace FastReport.Designer.ViewModels;

public partial class AppBootstrapperViewModel : AppViewModel
{
    #region Constructors

    public AppBootstrapperViewModel(IServiceProvider service) : base(service)
    {
        Title = "Avalonia Designer";
        Report = new Report();
        MenuHelper = Service.GetRequiredService<FastrReportDesignerMenuHelper>();
        WelcomeResult = new Interaction<WelcomeResult, Unit>();
        CanClose = new Interaction<Unit, bool>();
        
        var args = Environment.GetCommandLineArgs();
        if(args.Length < 2) return;
        
        try
        {
            Report.Load(args[1]);
        }
        catch (Exception)
        {
            Dialogs.ErroAsync("Erro ao carregar o report");
        }
    }

    #endregion Constructors

    #region Properties
    
    [Reactive]
    public partial Report Report { get; set; }
    
    public FastrReportDesignerMenuHelper MenuHelper { get; }
    
    public Interaction<WelcomeResult, Unit> WelcomeResult { get; }
    
    public Interaction<Unit, bool> CanClose { get; }

    #endregion Properties

    #region Methods

    [ReactiveCommand]
    private async Task PluginManager()
    {
        var resp = await Dialogs.ShowAsync<PluginManagerDialogViewModel, bool, DialogOptions>(new DialogOptions());
        if (resp == false) return;

        const string mensagem = "A aplicação precisa ser reiniciada para aplicar as alterações.";
        await Dialogs.WarnAsync(mensagem);
        RestartApp();
    }

    [ReactiveCommand]
    private async Task FinishApp()
    {
        if(!await CanClose.Handle(Unit.Default)) return;
        base.FinishApp();
    }

    public override async void ViewAppeared()
    {
        try
        {
            if(!Config.WelcomeEnabled) return;
        
            var ret = await Dialogs.ShowAsync<WelcomeDialogViewModel, WelcomeResult, DialogOptions>(new DialogOptions());
            await WelcomeResult.Handle(ret);
        }
        catch (Exception e)
        {
            Log.LogError(e, "Erro ao fechar aplicação");
        }
    }

    #endregion Methods
}