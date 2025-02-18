﻿using Avalonia;
using System;
using Avalonia.ReactiveUI;
using Caramelo.MvvmApp;
using Caramelo.MvvmApp.Avalonia.Extensions;
using Caramelo.MvvmApp.Extensions;
using FastReport.Designer.Commom;
using FastReport.Designer.Services;
using FastReport.Designer.ViewModels;
using FastReport.Designer.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

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
            .UseDialogResolver<DialogWindowResolver>()
            .UserSplash<AppSplashView, AppSplashViewModel>();

        // Registrando serviços
        builder.Services.AddTransient<PluginManagerService>();
        builder.Services.AddTransient<FastReportMenuLocalization>();
        builder.Services.AddSingleton<FastrReportDesignerMenuHelper>();
        
        builder.Logging.AddSerilog(new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File("Logs\\fastreport_designer.log", rollingInterval: RollingInterval.Day)
            .CreateLogger(), dispose: true);
        
        // Registrando View/ViewModel
        builder.Services.AddViewAndModelFromAssembly();
            
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