using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.CancelExpense
{
    public class CancelExpenseRequestObj : IRequestObj<CancelExpenseResponseObj>
    {
        public CancelExpenseRequestObj(IPresenterCallBack<CancelExpenseResponseObj> callBack, UserBobj currentUser, ExpenseBobj expenseToBeCancelled, CancellationToken cts)
        {
            CallBack = callBack;
            CurrentUser = currentUser;
            ExpenseToBeCancelled = expenseToBeCancelled;
            PresenterCallBack = callBack;
            Cts = cts;
        }

        public CancellationToken Cts { get; }

        public IPresenterCallBack<CancelExpenseResponseObj> PresenterCallBack { get; }

        public ExpenseBobj ExpenseToBeCancelled { get; }

        public UserBobj CurrentUser { get; }

       public IPresenterCallBack<CancelExpenseResponseObj> CallBack { get; }
    }
}
