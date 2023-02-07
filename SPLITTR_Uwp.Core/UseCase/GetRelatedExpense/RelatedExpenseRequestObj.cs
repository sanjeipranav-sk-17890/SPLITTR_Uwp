using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.GetRelatedExpense;

public class RelatedExpenseRequestObj :IRequestObj<RelatedExpenseResponseObj>
{
    public RelatedExpenseRequestObj(ExpenseBobj referenceExpense, UserBobj currentUser, CancellationToken cts, IPresenterCallBack<RelatedExpenseResponseObj> presenterCallBack)
    {
        ReferenceExpense = referenceExpense;
        CurrentUser = currentUser;
        Cts = cts;
        PresenterCallBack = presenterCallBack;
    }

    public ExpenseBobj ReferenceExpense { get; }

    public UserBobj CurrentUser { get; }

    public CancellationToken Cts { get; }

    public IPresenterCallBack<RelatedExpenseResponseObj> PresenterCallBack { get; }
}
