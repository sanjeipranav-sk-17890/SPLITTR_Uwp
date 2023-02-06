using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.CancelExpense
{
    public class CancelExpenseRequestObj : IRequestObj<CancelExpenseResponseObj>
    {
        public CancelExpenseRequestObj(IPresenterCallBack<CancelExpenseResponseObj> callBack, UserBobj currentUser, string expenseToBeCancelledId, CancellationToken cts)
        {
            CallBack = callBack;
            CurrentUser = currentUser;
            ExpenseToBeCancelledId = expenseToBeCancelledId;
            PresenterCallBack = callBack;
            Cts = cts;
        }

        public CancellationToken Cts { get; }

        public IPresenterCallBack<CancelExpenseResponseObj> PresenterCallBack { get; }

        public string ExpenseToBeCancelledId { get; }

        public UserBobj CurrentUser { get; }

       public IPresenterCallBack<CancelExpenseResponseObj> CallBack { get; }
    }
}
