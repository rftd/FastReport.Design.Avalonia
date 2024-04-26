using System.Collections.Generic;
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

    protected override void OnStarted(IEnumerable<string> args, bool isFirstInstance, AppBootstrapperViewModel appBootstrapper)
    {
        //
    }
}