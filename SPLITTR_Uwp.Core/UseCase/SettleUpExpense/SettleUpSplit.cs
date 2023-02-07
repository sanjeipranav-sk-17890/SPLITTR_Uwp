using System;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager.Contracts;

namespace SPLITTR_Uwp.Core.UseCase.SettleUpExpense
{
    public class SettleUpSplit : UseCaseBase<SettleUpExpenseResponseObj>
    {
        private readonly SettleUPExpenseRequestObj _requestObj;
        private readonly ISettleUpSplitDataManager _dataManager;
        public SettleUpSplit(SettleUPExpenseRequestObj requestObj,ISettleUpSplitDataManager dataManager) : base(requestObj.PresenterCallBack,requestObj.Cts)
        {
            _requestObj = requestObj;
            _dataManager = dataManager;
        }
        public override void Action()
        {
            _dataManager.SettleUpExpenses(_requestObj.SettleExpense,_requestObj.CurrentUser,new UseCaseCallBackBase<SettleUpExpenseResponseObj>(this),_requestObj.IsWalletTransaction);
        }
    }

}
