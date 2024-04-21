using Avalonia.Markup.Xaml;
using Caramelo.MvvmApp.Avalonia;
using FastReport.Designer.ViewModels;

namespace FastReport.Designer;

public partial class App : MvvmApplication<AppBootstrapperViewModel>
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}