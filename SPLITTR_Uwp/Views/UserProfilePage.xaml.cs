using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SPLITTR_Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserProfilePage : Page
    {
        private UserProfilePageViewModel _viewModel;
        public UserProfilePage()
        {
            _viewModel = App.Container.GetService<UserProfilePageViewModel>();
            this.InitializeComponent();
            _viewModel.BindingUpdateInvoked += _viewModel_BindingUpdateInvoked;
        }

        private void _viewModel_BindingUpdateInvoked()
        {
           Bindings.Update();
        }
    }
}
