using Caramelo.MvvmApp.Avalonia.Controls;
using FastReport.Designer.ViewModels;

namespace FastReport.Designer.Views;

public partial class WelcomeDialogView : MvvmWindow<WelcomeDialogViewModel>
{
    public WelcomeDialogView()
    {
        InitializeComponent();
    }
}