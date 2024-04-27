using System;
using System.Reactive;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;
using FastReport.Designer.Commom;
using FastReport.Utils;
using FastReport.Wizards;
using ReactiveUI;

namespace FastReport.Designer.ViewModels;

public class WelcomeDialogViewModel : MvvmDialogViewModel<DialogOptions, WelcomeResult>
{
    private bool showWelcome;

    #region Constructors

    public WelcomeDialogViewModel(IServiceProvider service) : base(service)
    {
        showWelcome = true;
        this.WhenAnyValue(x => x.ShowWelcome).Subscribe(x => Config.WelcomeEnabled = x);
        
        NewReportCommand = ReactiveCommand.Create<WelcomeType>(tipo =>
        {
            WizardBase? wizard = tipo switch
            {
                WelcomeType.NewBlank => Activator.CreateInstance<BlankReportWizard>(),
                WelcomeType.NewInherited => Activator.CreateInstance<InheritedReportWizard>(),
                WelcomeType.ReportWizard => Activator.CreateInstance<StandardReportWizard>(),
                WelcomeType.LabelWizard => Activator.CreateInstance<LabelWizard>(),
                _ => null
            };
            
            SetResult(new WelcomeResult { Tipo = tipo, Wizard = wizard });
        });
        
        RecentReportCommand = ReactiveCommand.Create<string>(file =>
        {
            SetResult(new WelcomeResult { Tipo = WelcomeType.Recent, File = file });
        });
    }

    #endregion Constructors

    #region Properties

    public string[] RecentFiles { get; private set; }

    public bool ShowWelcome
    {
        get => showWelcome;
        set => this.RaiseAndSetIfChanged(ref showWelcome, value);
    }

    public ReactiveCommand<WelcomeType, Unit> NewReportCommand { get; }
    
    public ReactiveCommand<string, Unit> RecentReportCommand { get; }

    #endregion Properties
    
    #region Methods

    public override void Initialize(DialogOptions parameter)
    {
        var recent = Config.Root.FindItem("Designer").GetProp("RecentFiles");
        if(string.IsNullOrEmpty(recent)) return;

        RecentFiles = recent.Split("\r", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    #endregion Methods
}