using System.Collections.Generic;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Adapters.SqlAdapter;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DbHandler;

public class ExpenseDbHandler : IExpenseDbHandler
{
    private readonly ISqlDataAdapter _sqlDbAccess;

    public  ExpenseDbHandler(ISqlDataAdapter sqlDbAccess)
    {
        _sqlDbAccess = sqlDbAccess;
        _sqlDbAccess.CreateTable<Expense>();
    }

    public Task<int> InsertExpenseAsync(Expense expense)
    {
        return _sqlDbAccess.InsertObj(expense);
    }
    public Task<int> InsertExpenseAsync(IEnumerable<Expense> expenses)
    {
        return _sqlDbAccess.InsertObjects(expenses);
    }

    public Task<int> UpdateExpenseAsync(Expense expense)
    {
        return _sqlDbAccess.UpdateObj(expense);
    }

    public async Task<IEnumerable<Expense>> SelectUserExpensesAsync(string userEmail)
    {
        return await _sqlDbAccess.FetchTable<Expense>()
            .Where(ex => ex.RequestedOwner.Equals(userEmail) || ex.UserEmailId.Equals(userEmail)).ToListAsync().ConfigureAwait(false);
    }
    public async Task<IEnumerable<Expense>> SelectRelatedExpenses(string expenseUniqueId)
    {
        return await _sqlDbAccess.FetchTable<Expense>().Where(ex => ex.ExpenseUniqueId.Equals(expenseUniqueId) || ex.ParentExpenseId.Equals(expenseUniqueId)).ToListAsync().ConfigureAwait(false);
    }
}