using System;
using System.Text;
using SPLITTR_Uwp.Core.DbHandler;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic.contracts;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic
{


    public interface IExpenseUseCase 
    {

        /// <summary>
        /// Event Raised when Respective Action Completed SuccessFully
        /// </summary>
        event Action<EventArgs> PresenterCallBackOnSuccess;

        /// <exception cref="ArgumentException">Exception thrown if Owner of Passed Expense Did'nt match Current User</exception>
        void CancelExpense(string expenseToBeCancelledId,UserBobj currentUser);

        void MarkExpenseAsPaid(string expenseToBeMarkedAsPaid, UserBobj currentUser);

    }

}
