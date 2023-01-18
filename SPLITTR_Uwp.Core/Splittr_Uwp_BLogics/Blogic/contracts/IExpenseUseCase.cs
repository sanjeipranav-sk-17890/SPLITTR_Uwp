using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic
{
    public interface IExpenseUseCase : IUseCase
    {

        /// <summary>
        /// Splits Expenses and populates remaining feilds in ExpenseBobjs ,and saves data to corresponding data services
        /// </summary>
        /// <param name="expenseDescription"></param>
        /// <param name="currentUser"></param>
        /// <param name="expenses"></param>
        /// <param name="expenseNote"></param>
        /// <param name="dateOfExpense"></param>
        /// <param name="expenseAmount"></param>
        /// <param name="expenditureSplitType">unequal Spilt: number>0 Equal Split <= 0 </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">throws exception when equal split option is selected but expense amount is less than or equal to 0,Or expediture amount is negative</exception>
        public void SplitNewExpensesAsync(string expenseDescription,UserBobj currentUser, IEnumerable<ExpenseBobj> expenses, string expenseNote, DateTime dateOfExpense, double expenseAmount, int expenditureSplitType);


        /// <summary>
        /// Event Raised when Respective Action Completed SuccessFully
        /// </summary>
        event Action<EventArgs> PresenterCallBackOnSuccess;


        void GetRelatedExpenses(ExpenseBobj referenceExpense, UserBobj currentUser, Action<IEnumerable<ExpenseBobj>> resultCallBack);


        /// <exception cref="ArgumentException">Exception thrown if Owner of Passed Expense Did'nt match Current User</exception>
        void CancelExpense(string expenseToBeCancelledId,UserBobj currentUser);

        void MarkExpenseAsPaid(string expenseToBeMarkedAsPaid, UserBobj currentUser);

    }
}
