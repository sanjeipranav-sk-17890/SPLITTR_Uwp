using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.Views
{
    public sealed partial class WalletBalanceUpdatePopUp : UserControl
    {
        
        private WalletBalanceUpdateViewModel _viewModel;
        public event Action MakePaymentClicked;

        public event Action CloseButtonClicked;
        
        public WalletBalanceUpdatePopUp()
        {
            this.InitializeComponent();
            _viewModel =  App.Container.GetService(typeof(WalletBalanceUpdateViewModel)) as WalletBalanceUpdateViewModel;
            _viewModel.CloseButtonClicked += CloseButtonClicked;
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.AddMoneyToWalletButtonClicked();
        }
        
        
    }
}
