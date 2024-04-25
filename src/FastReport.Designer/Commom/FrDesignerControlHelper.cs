using System;
using System.Reactive;
using FastReport.Design.StandardDesigner;
using FastReport.Designer.Extensions;
using ReactiveUI;

namespace FastReport.Designer.Commom;

public sealed class FrDesignerControlHelper
{
    #region Constructors

    public FrDesignerControlHelper(DesignerControl designerControl)
    {
        designerControl.MainMenu.miReport.DropDownItems[0].SetChecked(true);
        ReportTitle = designerControl.MainMenu.miReport.DropDownItems[0].ToCheckedObservable();
        ReportTitleCommand = ReactiveCommand.Create(() =>
        {
            designerControl.MainMenu.miReport.DropDownItems[0].ToggleChecked();
        });
        
        designerControl.MainMenu.miReport.DropDownItems[1].SetChecked(false);
        ReportSummary = designerControl.MainMenu.miReport.DropDownItems[1].ToCheckedObservable();
        ReportSummaryCommand = ReactiveCommand.Create(() =>
        {
            designerControl.MainMenu.miReport.DropDownItems[1].ToggleChecked();
        });
        
        designerControl.MainMenu.miReport.DropDownItems[2].SetChecked(true);
        ReportHeader = designerControl.MainMenu.miReport.DropDownItems[2].ToCheckedObservable();
        ReportHeaderCommand = ReactiveCommand.Create(() =>
        {
            designerControl.MainMenu.miReport.DropDownItems[2].ToggleChecked();
        });
        
        designerControl.MainMenu.miReport.DropDownItems[3].SetChecked(true);
        ReportFooter = designerControl.MainMenu.miReport.DropDownItems[3].ToCheckedObservable();
        ReportFooterCommand = ReactiveCommand.Create(() =>
        {
            designerControl.MainMenu.miReport.DropDownItems[3].ToggleChecked();
        });
        
        designerControl.MainMenu.miReport.DropDownItems[4].SetChecked(false);
        HeaderColumn = designerControl.MainMenu.miReport.DropDownItems[3].ToCheckedObservable();
        HeaderColumnCommand = ReactiveCommand.Create(() =>
        {
            designerControl.MainMenu.miReport.DropDownItems[4].ToggleChecked();
        });

        
        designerControl.MainMenu.miReport.DropDownItems[5].SetChecked(false);
        FooterColumn = designerControl.MainMenu.miReport.DropDownItems[5].ToCheckedObservable();
        FooterColumnCommand = ReactiveCommand.Create(() =>
        {
            designerControl.MainMenu.miReport.DropDownItems[5].ToggleChecked();
        });

        
        designerControl.MainMenu.miReport.DropDownItems[6].SetChecked(false);
        OverlayColumn = designerControl.MainMenu.miReport.DropDownItems[6].ToCheckedObservable();
        OverlayColumnCommand = ReactiveCommand.Create(() =>
        {
            designerControl.MainMenu.miReport.DropDownItems[6].ToggleChecked();
        });


        ConfigurarBandasCommand = ReactiveCommand.Create(() =>
        {
            designerControl.MainMenu.miReport.DropDownItems[8].PerformClick();
        });
        
        AssistenteGruposCommand = ReactiveCommand.Create(() =>
        {
            designerControl.MainMenu.miReport.DropDownItems[9].PerformClick();
        });
    }

    #endregion Constructors

    #region Properties

    public IObservable<bool> ReportTitle { get; }
    
    public IObservable<bool> ReportSummary { get; }
    
    public IObservable<bool> ReportHeader { get; }
    
    public IObservable<bool> ReportFooter { get; }
    
    public IObservable<bool> HeaderColumn { get; }
    
    public IObservable<bool> FooterColumn { get; }
    
    public IObservable<bool> OverlayColumn { get; }
    
    public ReactiveCommand<Unit, Unit> ReportTitleCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ReportSummaryCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ReportHeaderCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ReportFooterCommand { get; }
    
    public ReactiveCommand<Unit, Unit> HeaderColumnCommand { get; }
    
    public ReactiveCommand<Unit, Unit> FooterColumnCommand { get; }
    
    public ReactiveCommand<Unit, Unit> OverlayColumnCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ConfigurarBandasCommand { get; }
    
    public ReactiveCommand<Unit, Unit> AssistenteGruposCommand { get; }

    #endregion Properties
}