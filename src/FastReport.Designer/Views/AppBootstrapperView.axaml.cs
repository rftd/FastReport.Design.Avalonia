using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using AvaloniaEdit.Utils;
using Caramelo.MvvmApp.Avalonia.Controls;
using FastReport.Designer.Extensions;
using FastReport.Designer.ViewModels;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Application = Avalonia.Application;

namespace FastReport.Designer.Views;

public partial class AppBootstrapperView : MvvmWindow<AppBootstrapperViewModel>
{
    public AppBootstrapperView()
    {
        InitializeComponent();
        
        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, viewModel => viewModel.Report, view => view.DesignerControl.Report)
                .DisposeWith(disposables);

            ViewModel.OnRestartApp.Subscribe(x =>
            {
                if(Application.Current == null)
                    return;
                
                ((App)Application.Current).DesktopApp.Restart(x);
            });
        });

        this.Events().Loaded.Subscribe(_ =>
        {
            var helper = DesignerControl.GetHelper();
            TituloRelatorio.Command = helper.ReportTitleCommand;
            helper.ReportTitle.Subscribe(x => ChkTituloRelatorio.IsChecked = x);
            
            SumarioRelatorio.Command = helper.ReportSummaryCommand;
            helper.ReportSummary.Subscribe(x => ChkSumarioRelatorio.IsChecked = x);
            
            HeaderRelatorio.Command = helper.ReportHeaderCommand;
            helper.ReportHeader.Subscribe(x => ChkHeaderRelatorio.IsChecked = x);
            
            FooterRelatorio.Command = helper.ReportFooterCommand;
            helper.ReportFooter.Subscribe(x => ChkFooterRelatorio.IsChecked = x);
            
            HeaderColumnRelatorio.Command = helper.HeaderColumnCommand;
            helper.HeaderColumn.Subscribe(x => ChkHeaderColumnRelatorio.IsChecked = x);
            
            FooterColumnRelatorio.Command = helper.FooterColumnCommand;
            helper.FooterColumn.Subscribe(x => ChkFooterColumnRelatorio.IsChecked = x);
            
            OverlayRelatorio.Command = helper.OverlayColumnCommand;
            helper.OverlayColumn.Subscribe(x => ChkOverlayRelatorio.IsChecked = x);
            
            ConfigurarBandas.Command = helper.ConfigurarBandasCommand;
            AssistenteGrupos.Command = helper.AssistenteGruposCommand;
            
            DesignerControl.RestoreConfig();
            DesignerControl.StartAutoSave();
        });
        
        this.Events().Closing.Subscribe(e =>
        {
            DesignerControl.ParentWindowClosing(e);
            if (e.Cancel) return;
            
            DesignerControl.SaveConfig();
            DesignerControl.StopAutoSave();
        });
        
        ClientSizeProperty.Changed.Subscribe(_ =>
        {
            ContentArea.BorderThickness = WindowState == WindowState.Maximized ? 
                new Thickness(8,0,8,8) : new Thickness(0);
        });
    }
}