using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.SettleUpExpense;

public class SettleUPExpenseRequestObj : SplittrRequestBase<SettleUpExpenseResponseObj>
{
    public SettleUPExpenseRequestObj(bool isWalletTransaction, UserBobj currentUser, ExpenseBobj settleExpense, IPresenterCallBack<SettleUpExpenseResponseObj> presenterCallBack, CancellationToken cts) : base(cts,presenterCallBack)
    {
        IsWalletTransaction = isWalletTransaction;
        CurrentUser = currentUser;
        SettleExpense = settleExpense;
    }

    public ExpenseBobj SettleExpense { get; }

    public UserBobj CurrentUser { get; }

    public bool IsWalletTransaction { get; }

}