using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;
using System;
using System.Threading;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic.contracts;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.AddWalletAmount;
using SPLITTR_Uwp.Core.UseCase.UpdateUser;
using SQLite;

namespace SPLITTR_Uwp.ViewModel
{
    public class WalletBalanceUpdateViewModel : ObservableObject,IPresenterCallBack<UpdateUserResponseObj>
    {
        private string _moneyTextBoxText;
        private bool _invalidInputTextBlockVisibility;

        public event Action CloseButtonClicked;

        public UserViewModel UserViewModel { get; }

        public WalletBalanceUpdateViewModel()
        {
            UserViewModel = new UserViewModel(Store.CurreUserBobj);
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


        public  void AddMoneyToWalletButtonClicked()
        {
            //Wallet adding money should be non negative and a number
            if (double.TryParse(MoneyTextBoxText, out var newWalletBalance) && newWalletBalance > -1)
            {
                InvalidInputTextBlockVisibility = false;
                
                //Should Made Static invoke source if Previous Request Cancelled
                var cts = new CancellationTokenSource().Token;

                var addWalletAmountRequestObject = new AddWalletAmountRequestObject(this, cts, Store.CurreUserBobj, newWalletBalance);

                var addWalletAmountUseCase = InstanceHelper.CreateInstance<AddWalletAmount>(addWalletAmountRequestObject);

                addWalletAmountUseCase.Execute();
            }
            else
            {
                InvalidInputTextBlockVisibility = true;
            }

        }

        public async void OnSuccess(UpdateUserResponseObj result)
        {
            await UiService.RunOnUiThread(async () =>
            {
                await UiService.ShowContentAsync("Amount Added to Wallet SuccessFully", "Payment SuccessFull!!");
                CloseButtonClicked?.Invoke();
            }).ConfigureAwait(false);

        }
        public void OnError(SplittrException ex)
        {
            if (ex.InnerException is SQLiteException)
            {
                ExceptionHandlerService.HandleException(ex);
            }
        }
    }
}
