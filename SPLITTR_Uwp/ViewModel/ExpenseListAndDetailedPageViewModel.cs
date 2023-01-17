using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;
using User = SPLITTR_Uwp.Core.Models.User;

namespace SPLITTR_Uwp.ViewModel
{
    internal class ExpenseListAndDetailedPageViewModel : ObservableObject
    {
       
        private bool _ownerExpenseUserControlVisibility;
        private bool _owingMoneyPaymentControlVisibility;
        private ExpenseViewModel _controlDataContext;

        public bool OwnerExpenseUserControlVisibility
        {
            get => _ownerExpenseUserControlVisibility;
            set => SetProperty(ref _ownerExpenseUserControlVisibility, value);
        }

        public bool OwingMoneyPaymentControlVisibility
        {
            get => _owingMoneyPaymentControlVisibility;
            set => SetProperty(ref _owingMoneyPaymentControlVisibility, value);
        }

        public ExpenseViewModel SelectedExpenseObj { get; set; }

        public ExpenseViewModel ControlDataContext
        {
            get => _controlDataContext;
            set => SetProperty(ref _controlDataContext, value);
        }


        public void ExpenseSelectionMade()
        {
            if (SelectedExpenseObj is null)
            {
                return;
            }
            ChangeRespectiveControlVisibility(SelectedExpenseObj);
            ControlDataContext = SelectedExpenseObj;
        }

        private void ChangeRespectiveControlVisibility(ExpenseBobj expense)
        {
            if (expense.ExpenseStatus == ExpenseStatus.Pending && IsNotCurrentUser(expense.SplitRaisedOwner))
            {
                OwingMoneyPaymentControlVisibility = true;
                OwnerExpenseUserControlVisibility = !OwingMoneyPaymentControlVisibility;
            }
            else
            {
                OwnerExpenseUserControlVisibility = true;
                OwingMoneyPaymentControlVisibility = !OwnerExpenseUserControlVisibility;
            }
        }
        private bool IsNotCurrentUser(User user)
        {
            return !Store.CurreUserBobj.Equals(user);
        }


    }
}
