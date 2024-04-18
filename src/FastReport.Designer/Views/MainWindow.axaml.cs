using Avalonia;
using Avalonia.Controls;
using AvaloniaEdit.Utils;

namespace FastReport.Designer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DesignerControl.Report = new Report();
        Loaded += (_, _) =>
        {
            DesignerControl.RestoreConfig();
            DesignerControl.StartAutoSave();
        };
        
        Closing += (_, e) => 
        {
            DesignerControl.ParentWindowClosing(e);
            if (e.Cancel) return;
            
            DesignerControl.SaveConfig();
            DesignerControl.StopAutoSave();
        };

        this.GetObservable(ClientSizeProperty).Subscribe(_ =>
        {
            ContentArea.BorderThickness = WindowState == WindowState.Maximized ? 
                new Thickness(8,0,8,8) : new Thickness(0);
        });
    }
}