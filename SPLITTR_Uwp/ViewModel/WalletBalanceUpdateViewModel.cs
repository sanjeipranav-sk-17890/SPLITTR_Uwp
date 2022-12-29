using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;
using System;

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

        public WalletBalanceUpdateViewModel(IUserUtility userUtility, DataStore store)
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
                await UiService.ShowContentAsync("Amount Added to Wallet SuccessFully", "Payment SuccessFull!!");
                CloseButtonClicked?.Invoke();
            }
            else
            {
                InvalidInputTextBlockVisibility = true;
            }

        }



    }
}
