using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.Services.Contracts
{
    public interface IExpenseDataServices
    {
        /// <summary>
        /// Inserts Given Expense object to DataService Provided
        /// </summary>
        /// <param name="expense"></param>
        /// <returns></returns>
        Task<int> InsertExpenseAsync(Expense expense);

        /// <summary>
        /// updates Given expense object based on expense Unique Id in Corresponding Dataservice Provider
        /// </summary>
        /// <param name="expense"></param>
        /// <returns></returns>
        Task<int> UpdateExpenseAsync(Expense expense);

        /// <summary>
        /// Returns Expenses of particular User's based on User Unique Attribute
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        Task<IEnumerable<Expense>> SelectUserExpensesAsync(string userEmail);

    }
}
