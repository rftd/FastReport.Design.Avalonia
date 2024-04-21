using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using AvaloniaEdit.Utils;
using Caramelo.MvvmApp.Avalonia.Controls;
using FastReport.Designer.ViewModels;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

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
        });

        this.Events().Loaded.Subscribe(_ =>
        {
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

        this.GetObservable(ClientSizeProperty).Subscribe(_ =>
        {
            ContentArea.BorderThickness = WindowState == WindowState.Maximized ? 
                new Thickness(8,0,8,8) : new Thickness(0);
        });
    }
}