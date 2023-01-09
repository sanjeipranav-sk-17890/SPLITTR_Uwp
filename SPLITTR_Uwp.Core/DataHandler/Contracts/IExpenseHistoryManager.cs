using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;

namespace SPLITTR_Uwp.Core.DataHandler.Contracts
{
    public interface IExpenseHistoryManager : IUseCase
    {
        /// <summary>
        /// stores History of expenses whether it is marked as Paid in Respective DataService
        /// </summary>
        /// <param name="expense"></param>
        /// <returns></returns>
        Task RecordExpenseMarkedAsPaid(string expenseId);

        Task<bool> IsExpenseMarkedAsPaid(string expenseId);
    }
}
