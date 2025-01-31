using System;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;
using FastReport.Designer.Commom;
using FastReport.Utils;
using FastReport.Wizards;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace FastReport.Designer.ViewModels;

public partial class WelcomeDialogViewModel : MvvmDialogViewModel<DialogOptions, WelcomeResult>
{
    #region Constructors

    public WelcomeDialogViewModel(IServiceProvider service) : base(service)
    {
        ShowWelcome = Config.WelcomeEnabled;
        this.WhenAnyValue(x => x.ShowWelcome)
            .Subscribe(x => Config.WelcomeEnabled = x);
    }

    #endregion Constructors
    
    #region Properties

    [Reactive]
    public partial bool ShowWelcome { get; set; }
    
    [Reactive]
    public partial  string[] RecentFiles { get; set; }

    #endregion Properties
    
    #region Methods

    public override void Initialize(DialogOptions parameter)
    {
        var recent = Config.Root.FindItem("Designer").GetProp("RecentFiles");
        if(string.IsNullOrEmpty(recent)) return;

        RecentFiles = recent.Split("\r", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    [ReactiveCommand]
    private void NewReport(WelcomeType tipo)
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
    }

    [ReactiveCommand]
    private void RecentReport(string file) => SetResult(new WelcomeResult { Tipo = WelcomeType.Recent, File = file });
    
    #endregion Methods
}