using System;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.CancelExpense;

namespace SPLITTR_Uwp.Core.DataManager.Contracts
{


    public interface IExpenseCancellationDataManager
    {
        /// <exception cref="ArgumentException">Exception thrown if Owner of Passed Expense Did'nt match Current User</exception>
        void CancelExpense(ExpenseBobj expenseToBeCancelledId, UserBobj currentUser,IUseCaseCallBack<CancelExpenseResponseObj> callBack);
    }

}
