using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;

namespace SPLITTR_Uwp.Core.UseCase.CancelExpense
{
    public class CancelExpense :UseCaseBase<CancelExpenseResponseObj>
    {
        private readonly CancelExpenseRequestObj _requestObj;
        private readonly IExpenseCancellationDataManager _dataManager;
        public CancelExpense(CancelExpenseRequestObj requestObj,IExpenseCancellationDataManager dataManager) : base(requestObj.CallBack,requestObj.Cts)
        {
            _requestObj = requestObj;
            _dataManager = dataManager;
        }
        public override void Action()
        {
            _dataManager.CancelExpense(_requestObj.ExpenseToBeCancelledId,_requestObj.CurrentUser,new UseCaseCallBackBase<CancelExpenseResponseObj>(this));
        }
    }
}
