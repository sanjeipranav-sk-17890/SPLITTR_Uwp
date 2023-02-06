using System.Collections.Generic;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DataManager.Contracts
{
    public interface IExpenseDataManager
    {
        Task InsertExpenseAsync(IEnumerable<ExpenseBobj> expenseBobjs);
        Task UpdateExpenseAsync(ExpenseBobj expenseBobj);
        Task<IEnumerable<ExpenseBobj>> GetUserExpensesAsync(User user,IUserDataManager userDataManager);
    }
}
