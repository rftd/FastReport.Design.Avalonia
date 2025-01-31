using System;
using System.ComponentModel;
using Avalonia;
using System.Reactive.Disposables;
using Caramelo.MvvmApp.Avalonia;
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
            ViewModel.MenuHelper.AvaloniaDesigner = DesignerControl;

            this.Bind(ViewModel, viewModel => viewModel.Report, view => view.DesignerControl.Report)
                .DisposeWith(disposables);

            ViewModel.OnRestartApp.Subscribe(x =>
            {
                if (Application.Current == null)
                    return;

                ((App)Application.Current).DesktopApp.Restart(x);
            }).DisposeWith(disposables);

            ViewModel.WelcomeResult.RegisterHandler(x =>
            {
                switch (x.Input.Tipo)
                {
                    case WelcomeType.Open:
                        DesignerControl.cmdOpen.Execute(null);
                        break;

                    case WelcomeType.Recent:
                        DesignerControl.cmdOpen.LoadFile(x.Input.File);
                        break;

                    default:
                        x.Input.Wizard?.Run(DesignerControl.InnerDesigner);
                        break;
                }
            }).DisposeWith(disposables);

            ViewModel.CanClose.RegisterHandler(x =>
            {
                var e = new CancelEventArgs();
                DesignerControl.ParentWindowClosing(e);
                if (e.Cancel) x.SetOutput(false);

                DesignerControl.SaveConfig();
                DesignerControl.StopAutoSave();
                x.SetOutput(true);
            }).DisposeWith(disposables);
        });

        this.Events().Loaded.Subscribe(_ =>
        {
            ViewModel.MenuHelper.GenerateMenu(MainMenu, ViewModel);

            DesignerControl.RestoreConfig();
            DesignerControl.StartAutoSave();
        });
    }
}