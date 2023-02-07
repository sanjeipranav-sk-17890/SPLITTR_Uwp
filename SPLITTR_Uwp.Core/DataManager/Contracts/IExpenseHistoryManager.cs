using SPLITTR_Uwp.Core.DataManager;
using System;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.VerifyPaidExpense;

namespace SPLITTR_Uwp.Core.DataManager.Contracts
{
    public interface IExpenseHistoryManager 
    {
        /// <summary>
        /// stores History of expenses whether it is marked as Paid in Respective DataService
        /// </summary>
        /// <param name="expense"></param>
        /// <returns></returns>
        void RecordExpenseMarkedAsPaid(string expenseId);

        void IsExpenseMarkedAsPaid(string expenseId,IUseCaseCallBack<VerifyPaidExpenseResponseObj> callBack);

    }
}
