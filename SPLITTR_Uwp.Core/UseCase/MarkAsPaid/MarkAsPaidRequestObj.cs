using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.MarkAsPaid
{
    public class MarkAsPaidRequestObj : IRequestObj<MarkAsPaidResponseObj>
    {
        public MarkAsPaidRequestObj(IPresenterCallBack<MarkAsPaidResponseObj> presenterCallBack, CancellationToken cts, UserBobj userBobj, ExpenseBobj expenseToBeMarkedAsPaid)
        {
            PresenterCallBack = presenterCallBack;
            Cts = cts;
            ExpenseToBeMarkedAsPaid = expenseToBeMarkedAsPaid;
            CurrentUser = userBobj;
            ExpenseToBeMarkedAsPaid = expenseToBeMarkedAsPaid;
        }

        public CancellationToken Cts { get; }

        public IPresenterCallBack<MarkAsPaidResponseObj> PresenterCallBack { get; }

        public ExpenseBobj ExpenseToBeMarkedAsPaid { get; }

        public UserBobj CurrentUser { get; }

    }
}
