using System.ComponentModel;
using System.Runtime.CompilerServices;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
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

        public new string LentAmount
        {
            get => _user.LentAmount.ExpenseAmount(_user);
        }

        public new string PendingAmount
        {
            get => _user.PendingAmount.ExpenseAmount(_user);
        }


        public new int CurrencyPreference
        {
            get => (int)_user.CurrencyPreference;
            set => _user.CurrencyPreference = (Currency)value;
        }

        

        public UserViewModel(UserBobj user) : base(user)
        {
            _user = user;
            _user.ValueChanged += InnerObjValueChanged;
        }

        public void InnerObjValueChanged(string property)
        {
            OnPropertyChanged(property);
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
