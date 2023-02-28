using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.CancelExpense;

public class CancelExpenseRequestObj : SplittrRequestBase<CancelExpenseResponseObj>
{
    public CancelExpenseRequestObj(IPresenterCallBack<CancelExpenseResponseObj> callBack, UserBobj currentUser, ExpenseBobj expenseToBeCancelled, CancellationToken cts) : base(cts,callBack)
    {
    
        CurrentUser = currentUser;
        ExpenseToBeCancelled = expenseToBeCancelled;
       
    }

    public ExpenseBobj ExpenseToBeCancelled { get; }

    public UserBobj CurrentUser { get; }

}