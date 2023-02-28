using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.GetRelatedExpense;

public class RelatedExpenseRequestObj :SplittrRequestBase<RelatedExpenseResponseObj>
{
    public RelatedExpenseRequestObj(ExpenseBobj referenceExpense, UserBobj currentUser, CancellationToken cts, IPresenterCallBack<RelatedExpenseResponseObj> presenterCallBack) :base(cts, presenterCallBack)
    {
        ReferenceExpense = referenceExpense;
        CurrentUser = currentUser;
        
    }

    public ExpenseBobj ReferenceExpense { get; }

    public UserBobj CurrentUser { get; }

}
