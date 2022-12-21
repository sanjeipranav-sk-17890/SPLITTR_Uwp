using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic
{
    public interface IExpenseUtility
    {

        /// <summary>
        /// Splits Expenses and populates remaining feilds in ExpenseBobjs ,and saves data to corresponding data services
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="expenses"></param>
        /// <param name="expenseNote"></param>
        /// <param name="dateOfExpense"></param>
        /// <param name="expenseAmount"></param>
        /// <param name="expenditureSplitType">unequal Spilt: number>0 Equal Split <= 0 </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">throws exception when equal split option is selected but expense amount is less than or equal to 0,Or expediture amount is negative</exception>
        public Task SplitNewExpensesAsync(UserBobj currentUser, IEnumerable<ExpenseBobj> expenses, string expenseNote, DateTime dateOfExpense, double expenseAmount, int expenditureSplitType);


        /// <summary>
        /// Event Raised when Respective Action Completed SuccessFully
        /// </summary>
        event Action<EventArgs> PresenterCallBackOnSuccess;

    }
}
