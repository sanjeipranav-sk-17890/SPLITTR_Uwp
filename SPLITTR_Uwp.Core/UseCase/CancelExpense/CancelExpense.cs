using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.DependencyInjector;

namespace SPLITTR_Uwp.Core.UseCase.CancelExpense
{
    public class CancelExpense :UseCaseBase<CancelExpenseResponseObj>
    {
        private readonly CancelExpenseRequestObj _requestObj;
        private readonly IExpenseCancellationDataManager _dataManager;
        public CancelExpense(CancelExpenseRequestObj requestObj) : base(requestObj.CallBack,requestObj.Cts)
        {
            _requestObj = requestObj;
            _dataManager = SplittrDependencyService.GetInstance<IExpenseCancellationDataManager>();
        }
        public override void Action()
        {
            _dataManager.CancelExpense(_requestObj.ExpenseToBeCancelledId,_requestObj.CurrentUser,new UseCaseCallBackBase<CancelExpenseResponseObj>(this));
        }
    }
}
