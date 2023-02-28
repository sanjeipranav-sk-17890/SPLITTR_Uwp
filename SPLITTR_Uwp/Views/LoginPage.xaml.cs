using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SPLITTR_Uwp.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page
{
    private LoginPageViewModel _viewModel;
    public LoginPage()
    {
        InitializeComponent();
        _viewModel = App.Container.GetService<LoginPageViewModel>();
        DataContextChanged += (sender, args) => Bindings.Update();

    }
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        _viewModel.PageUnloaded();
    }

    public void SignUpButtonOnClick()
    {
        NavigationService.Frame.Navigate(typeof(SignUpPage), null, infoOverride: new SlideNavigationTransitionInfo
            { Effect = SlideNavigationTransitionEffect.FromRight });
    }

}