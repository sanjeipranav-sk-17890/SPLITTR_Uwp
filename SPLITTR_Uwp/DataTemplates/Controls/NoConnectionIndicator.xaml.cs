using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static SPLITTR_Uwp.Services.UiService;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates.Controls
{
    public sealed partial class NoConnectionIndicator : UserControl
    {
        public NoConnectionIndicator()
        {
            Loaded += NoConnectionIndicator_Loaded;
            NetworkInformation.NetworkStatusChanged += NetworkInformationOnNetworkStatusChanged;
            InitializeComponent();
            //Subscribing For Change in Network Connection
        }

        private void NoConnectionIndicator_Loaded(object sender, RoutedEventArgs e)
        {
            //Initial Checking For Network Connection
            NetworkInformationOnNetworkStatusChanged(default);
        }


        private void NetworkInformationOnNetworkStatusChanged(object sender)
        {
            var netInfo = NetworkInformation.GetInternetConnectionProfile();
            var level = netInfo?.GetNetworkConnectivityLevel();
            if (level is null or NetworkConnectivityLevel.None)
            {
              ShowNoConnectionIndicator();
                return;
            }
            HideNoConnectionIndicator();
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
