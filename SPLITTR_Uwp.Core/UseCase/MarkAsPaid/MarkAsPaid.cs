using SPLITTR_Uwp.Core.ModelBobj;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SPLITTR_Uwp.Core.UseCase.MarkAsPaid
{
    public class MarkAsPaid :UseCaseBase<MarkAsPaidResponseObj>
    {
        private readonly MarkAsPaidRequestObj _requestObj;
        private readonly IMarkExpensePaidDataManager _dataManager;

        public MarkAsPaid(MarkAsPaidRequestObj requestObj,IMarkExpensePaidDataManager dataManager) : base(requestObj.PresenterCallBack,requestObj.Cts)
        {
            _requestObj = requestObj;
            _dataManager = dataManager;
        }
        public override void Action()
        {
           _dataManager.MarkExpenseAsPaid(_requestObj.ExpenseIdToBeMarkedAsPaid,_requestObj.CurrentUser,new UseCaseCallBackBase<MarkAsPaidResponseObj>(this));
        }
    }
    public interface IMarkExpensePaidDataManager
    {
        void MarkExpenseAsPaid(string expenseToBeMarkedAsPaid, UserBobj currentUser,IUseCaseCallBack<MarkAsPaidResponseObj> callBack);

    }
}
