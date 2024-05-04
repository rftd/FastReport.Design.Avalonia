using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AvaloniaEdit.Utils;
using Caramelo.MvvmApp.Avalonia.Controls;
using FastReport.Designer.ViewModels;
using Material.Icons;
using Material.Icons.Avalonia;
using ReactiveMarbles.ObservableEvents;

namespace FastReport.Designer.Views;

public partial class WelcomeDialogView : MvvmWindow<WelcomeDialogViewModel>
{
    public WelcomeDialogView()
    {
        InitializeComponent();

        this.Events().Loaded.Subscribe(_ =>
        { 
            foreach (var recentFile in ViewModel.RecentFiles)
            {
                RecentFiles.Children.Add(new Button
                {
                    BorderThickness = Thickness.Parse("0"),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Command = ViewModel.RecentReportCommand,
                    CommandParameter = recentFile,
                    Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new MaterialIcon {Kind = MaterialIconKind.FileDocumentBox},
                            new TextBlock {Text = Path.GetFileName(recentFile)}
                        }
                    }
                });
            }
        });
    }
}