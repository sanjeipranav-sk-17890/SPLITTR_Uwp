using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SPLITTR_Uwp.Services;
using static SPLITTR_Uwp.Services.UiService;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls
{
    public sealed partial class NoConnectionIndicator : UserControl
    {
        public NoConnectionIndicator()
        {
            //Subscribing For Change in Network Connection
            NetworkInfoService.NetWorkConnectionChanged += NetworkInfoService_NetWorkConnectionChanged;
            InitializeComponent();
            Loaded += NoConnectionIndicator_Loaded;
            Unloaded += NoConnectionIndicator_Unloaded;
        }

        private void NoConnectionIndicator_Unloaded(object sender, RoutedEventArgs e)
        {
            NetworkInfoService.NetWorkConnectionChanged -= NetworkInfoService_NetWorkConnectionChanged;
        }

        private void NoConnectionIndicator_Loaded(object sender, RoutedEventArgs e)
        {
            SwitchVisibilityBasedOnBool(NetworkInfoService.IsNetworkConnectionAvailable);
        }

        private void NetworkInfoService_NetWorkConnectionChanged(NetWorkConnectionChangedEventArgs args)
        {
            SwitchVisibilityBasedOnBool(args.IsInterNetAvailable);
        }
        private void SwitchVisibilityBasedOnBool(bool isInternetAvailable)
        {

            if (isInternetAvailable)
            {
              HideNoConnectionIndicator();
                return;
            }
            ShowNoConnectionIndicator();
        }

        private async void HideNoConnectionIndicator()
        {
            await RunOnUiThread((() =>
            {
                Visibility = Visibility.Collapsed;
            })).ConfigureAwait(false);
        }
        private async void ShowNoConnectionIndicator()
        {
            await RunOnUiThread((() =>
            {
                Visibility = Visibility.Visible;
            })).ConfigureAwait(false);
        }

    }
}
