using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FastReport.Designer.Views;
using FastReport.Utils;

namespace FastReport.Designer;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var splashScreen = new SplashScreen();
            desktop.MainWindow = splashScreen;
                
            splashScreen.Show();
            
            await Task.Delay(1000);
            
            // Applying default settings
            Config.Folder = "";
            Config.UIStyle = UIStyle.Light;
            desktop.MainWindow = new MainWindow();
            desktop.MainWindow.Show();
            
            // Get rid of the splash screen
            splashScreen.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }
}