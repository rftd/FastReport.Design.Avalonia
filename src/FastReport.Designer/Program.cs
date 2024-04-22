using Avalonia;
using System;
using Avalonia.ReactiveUI;
using Caramelo.MvvmApp;
using Caramelo.MvvmApp.Avalonia.Extensions;
using Caramelo.MvvmApp.Extensions;
using FastReport.Designer.Services;
using FastReport.Designer.ViewModels;
using FastReport.Designer.Views;
using Microsoft.Extensions.DependencyInjection;

namespace FastReport.Designer;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main() 
    {
        var builder = MvvmApp.CreateBuilder();
        builder.UseAvalonia<App, AppBootstrapperViewModel, AppBootstrapperView>()
            .UserSplash<AppSplashView, AppSplashViewModel>();

        builder.Services.AddTransient<FrPluginManager>();
        
        // Registrando View/ViewModel
        builder.Services.AddViewTransient<PluginManagerDialogView, PluginManagerDialogViewModel>();
            
        var app = builder.Build();
        app.Run();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .UseReactiveUI()
            .LogToTrace();
}