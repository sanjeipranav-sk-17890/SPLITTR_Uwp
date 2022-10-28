using SPLITTR_Uwp.Core.Utility.Blogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;

namespace SPLITTR_Uwp.ViewModel
{
    public class WalletBalanceUpdateViewModel : ObservableObject
    {
        private readonly IUserUtility _userUtility;
        private string _moneyTextBoxText;
        private bool _invalidInputTextBlockVisibility;
        private DataStore _store;

        public event Action CloseButtonClicked;

        public UserViewModel UserViewModel { get; }

        public WalletBalanceUpdateViewModel(IUserUtility userUtility,DataStore store)
        {
            _userUtility = userUtility;
            _store = store;
            UserViewModel = new UserViewModel(_store.UserBobj);
        }

        public string MoneyTextBoxText
        {
            get => _moneyTextBoxText;
            set => SetProperty(ref _moneyTextBoxText, value);
        }

        public bool InvalidInputTextBlockVisibility
        {
            get => _invalidInputTextBlockVisibility;
            set => SetProperty(ref _invalidInputTextBlockVisibility, value);
        }


        public async void AddMoneyToWalletButtonClicked()
        {
            //Wallet adding money should be non negative and a number
            if (double.TryParse(MoneyTextBoxText, out var newWalletBalance) && newWalletBalance > -1)
            {
                InvalidInputTextBlockVisibility = false;
                await _userUtility.UpdateUserObjAsync(_store.UserBobj, newWalletBalance);
                await ShowMessageBoxAsync("Amount Added to Wallet SuccessFully", "Payment SuccessFull!!");
                CloseButtonClicked?.Invoke();
            }
            else
            {
                InvalidInputTextBlockVisibility = true;
            }

        }

        private async Task ShowMessageBoxAsync(string content, string title)
        {

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    MessageDialog msg = new MessageDialog(content, title);

                    msg.Commands.Add(new UICommand("close"));
                    await msg.ShowAsync();

                });

        }
       
    }
}
