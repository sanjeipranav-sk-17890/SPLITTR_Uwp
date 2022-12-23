using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using Windows.UI.Core;
using SPLITTR_Uwp.Services;

namespace SPLITTR_Uwp.ViewModel.Models
{
    public class UserViewModel : UserBobj, INotifyPropertyChanged
    {
        private readonly UserBobj _user;


        public override string UserName
        {
            get => _user.UserName;
        }

        public override double StrWalletBalance
        {
            get => _user.StrWalletBalance;
        }

        public string CurrentUserWalletBalance
        {
            get => _user.StrWalletBalance.ExpenseAmount(_user);
        }

        public new string LendedAmount
        {
            get => _user.LendedAmount.ExpenseAmount(_user);
        }

        public new string PendingAmount
        {
            get => _user.PendingAmount.ExpenseAmount(_user);
        }


        public new int CurrencyPreference
        {
            get
            {
                return (int)_user.CurrencyPreference;
            }
            set
            {
                _user.CurrencyPreference = (Currency)value;
            }
        }


        public UserViewModel(UserBobj user) : base(user)
        {
            _user = user;
            _user.ValueChanged += InnerObjValueChanged;
        }

        public void InnerObjValueChanged()
        {
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(CurrentUserWalletBalance));
            OnPropertyChanged(nameof(LendedAmount));
            OnPropertyChanged(nameof(PendingAmount));
            OnPropertyChanged(nameof(CurrencyPreference));
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(Expenses));
            OnPropertyChanged(nameof(Groups));

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected async virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            await UiService.RunOnUiThread(
                () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
        }
    }
}
