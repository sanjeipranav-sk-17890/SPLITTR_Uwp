using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;

namespace SPLITTR_Uwp.ViewModel;

internal class ExpenseListAndDetailedPageViewModel : ObservableObject
{
       
    private bool _ownerExpenseUserControlVisibility;
    private bool _owingMoneyPaymentControlVisibility;
    private ExpenseVobj _controlDataContext;

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

    public ExpenseVobj SelectedExpenseObj { get; set; }

    public ExpenseVobj ControlDataContext
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
        return !Store.CurrentUserBobj.Equals(user);
    }


}