using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.SettleUpExpense
{
    public class SettleUPExpenseRequestObj : IRequestObj<SettleUpExpenseResponseObj>
    {
        public SettleUPExpenseRequestObj(bool isWalletTransaction, UserBobj currentUser, ExpenseBobj settleExpense, IPresenterCallBack<SettleUpExpenseResponseObj> presenterCallBack, CancellationToken cts)
        {
            IsWalletTransaction = isWalletTransaction;
            CurrentUser = currentUser;
            SettleExpense = settleExpense;
            PresenterCallBack = presenterCallBack;
            Cts = cts;
        }

        public CancellationToken Cts { get; }

        public IPresenterCallBack<SettleUpExpenseResponseObj> PresenterCallBack { get; }

        public ExpenseBobj SettleExpense { get; }

        public UserBobj CurrentUser { get; }

        public bool IsWalletTransaction { get; }

    }
}
