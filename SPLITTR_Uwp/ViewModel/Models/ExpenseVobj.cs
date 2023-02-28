using System.ComponentModel;
using System.Runtime.CompilerServices;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Services;

namespace SPLITTR_Uwp.ViewModel.Models
{
    public class ExpenseVobj : ExpenseBobj,INotifyPropertyChanged
    {
       
        private bool _visibility = true;

        public override  ExpenseStatus ExpenseStatus
        {
            get => base.ExpenseStatus;
            set
            {
                if (base.ExpenseStatus == value)
                {
                    return;
                }
                base.ExpenseStatus = value;
                OnPropertyChanged();
            } 
        }

        public bool Visibility
        {
            get => _visibility ;
            set
            {
                if (value == _visibility)
                    return;
                _visibility = value;
                OnPropertyChanged();
            }
        }


        public ExpenseVobj(ExpenseBobj expense) : base(expense)
        {
            SplittrNotification.ExpenseStatusChanged += SplittrNotification_ExpenseStatusChanged;
        }

        private void SplittrNotification_ExpenseStatusChanged(ExpenseStatusChangedEventArgs obj)
        {
            if (obj?.StatusChangedExpense?.ExpenseUniqueId.Equals(ExpenseUniqueId) is true)
            {
                ExpenseStatus = obj.StatusChangedExpense.ExpenseStatus;
            }
        }

        #region INotifyPropertyChanged Region

        private void InnerObjValueChanged(string property)
        {
            OnPropertyChanged(property);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected async virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            await UiService.RunOnUiThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        #endregion

    }
}
