using System;
using System.Threading.Tasks;
using Caramelo.MvvmApp.ViewModel;

namespace FastReport.Designer.ViewModels;

public class AppSplashViewModel : MvvmSplashViewModel
{
    #region Constructors

    public AppSplashViewModel(IServiceProvider service) : base(service)
    {
    }

    #endregion Constructors

    #region Methods

    public override async void ViewAppeared()
    {
        await Task.Delay(3000);
        CloseSplash();
    }

    #endregion Methods
}