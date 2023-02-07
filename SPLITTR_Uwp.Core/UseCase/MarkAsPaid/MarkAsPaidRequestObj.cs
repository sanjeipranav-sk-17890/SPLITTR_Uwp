using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.MarkAsPaid
{
    public class MarkAsPaidRequestObj : IRequestObj<MarkAsPaidResponseObj>
    {
        public MarkAsPaidRequestObj(IPresenterCallBack<MarkAsPaidResponseObj> presenterCallBack, CancellationToken cts, string expenseIdToBeMarkedAsPaid, UserBobj userBobj)
        {
            PresenterCallBack = presenterCallBack;
            Cts = cts;
            ExpenseIdToBeMarkedAsPaid = expenseIdToBeMarkedAsPaid;
            CurrentUser = userBobj;
        }

        public CancellationToken Cts { get; }

        public IPresenterCallBack<MarkAsPaidResponseObj> PresenterCallBack { get; }

        public string ExpenseIdToBeMarkedAsPaid { get; }

        public UserBobj CurrentUser { get; }

    }
}
