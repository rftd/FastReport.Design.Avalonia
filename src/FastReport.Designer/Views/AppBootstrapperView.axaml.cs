using System;
using Avalonia;
using Avalonia.Controls;
using System.Reactive.Disposables;
using Caramelo.MvvmApp.Avalonia.Controls;
using FastReport.Designer.Commom;
using FastReport.Designer.Extensions;
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

            ViewModel.OnRestartApp.Subscribe(x =>
            {
                if(Application.Current == null)
                    return;
                
                ((App)Application.Current).DesktopApp.Restart(x);
            }).DisposeWith(disposables);

            ViewModel.OnWelcomeDone.Subscribe(result =>
            {
                switch (result.Tipo)
                {
                    case WelcomeType.Open:
                        DesignerControl.cmdOpen.Execute(null);
                        break;
                    
                    case WelcomeType.Recent:
                        DesignerControl.cmdOpen.LoadFile(result.File);
                        break;
                    
                    default:
                        result.Wizard?.Run(DesignerControl.InnerDesigner);
                        break;
                }
            }).DisposeWith(disposables);
        });

        this.Events().Loaded.Subscribe(_ =>
        {
            var helper = DesignerControl.GetMenuHelper();
            helper.GenerateMenu(MainMenu, ViewModel);
            
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