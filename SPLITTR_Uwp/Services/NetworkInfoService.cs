using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace SPLITTR_Uwp.Services
{
    internal static class NetworkInfoService
    {
        static NetworkInfoService()
        {
            NetworkInformation.NetworkStatusChanged += NetworkInformationOnNetworkStatusChanged;

        }

        public static bool IsNetworkConnectionAvailable
        {
            get => CheckIfNetworkConnectionAvailable();
        }

        public static event Action<NetWorkConnectionChangedEventArgs> NetWorkConnectionChanged;

        private static void NetworkInformationOnNetworkStatusChanged(object sender)
        {
            if (CheckIfNetworkConnectionAvailable())
            {
                InvokeConnectionEvent();
                return;
            }
            InvokeNoConnectionEvent();
        }

        private static bool CheckIfNetworkConnectionAvailable()
        {

            var netInfo = NetworkInformation.GetInternetConnectionProfile();
            var level = netInfo?.GetNetworkConnectivityLevel();
            return level is not (null or NetworkConnectivityLevel.None);
        }
        private static void InvokeNoConnectionEvent()
        {
            NetWorkConnectionChanged?.Invoke(new NetWorkConnectionChangedEventArgs(false));
        }
        private static void InvokeConnectionEvent()
        {
            NetWorkConnectionChanged?.Invoke(new NetWorkConnectionChangedEventArgs(true));
        }


    }
    internal class NetWorkConnectionChangedEventArgs : EventArgs
    {
        public bool IsInterNetAvailable { get; }

        public NetWorkConnectionChangedEventArgs(bool isInterNetAvailable)
        {
            IsInterNetAvailable = isInterNetAvailable;
        }

    }
}
