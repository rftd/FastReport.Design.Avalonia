using System;
using System.Threading.Tasks;
using Caramelo.MvvmApp.ViewModel;
using ReactiveUI;

namespace FastReport.Designer.ViewModels;

public class AppSplashViewModel : MvvmSplashViewModel
{
    #region Fields

    private string message;

    #endregion Fields

    #region Constructors

    public AppSplashViewModel(IServiceProvider service) : base(service)
    {
        message = "Iniciando o Designer";
    }

    #endregion Constructors

    #region Properties

    public string Message
    {
        get => message;
        set => this.RaiseAndSetIfChanged(ref message, value);
    }

    #endregion Properties

    #region Methods

    public override async void ViewAppeared()
    {
        await Task.Delay(3000);
        CloseSplash();
    }

    #endregion Methods
}