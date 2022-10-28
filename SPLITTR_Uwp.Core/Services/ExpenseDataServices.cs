using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Services.Contracts;
using SPLITTR_Uwp.Core.Services.SqliteConnection;

namespace SPLITTR_Uwp.Core.Services
{
    public class ExpenseDataServices : IExpenseDataServices
    {
        private readonly ISqlDataServices _sqlDbAccess;

        public  ExpenseDataServices(ISqlDataServices sqlDbAccess)
        {
            _sqlDbAccess = sqlDbAccess;
            _sqlDbAccess.CreateTable<Expense>();
        }

        public Task<int> InsertExpenseAsync(Expense expense)
        {
           return _sqlDbAccess.InsertObj(expense);
        }

        public Task<int> UpdateExpenseAsync(Expense expense)
        {
            return _sqlDbAccess.UpdateObj(expense);
        }

        public async Task<IEnumerable<Expense>> SelectUserExpensesAsync(string userEmail)
        {
          return await _sqlDbAccess.FetchTable<Expense>()
                .Where(ex => ex.RequestedOwner.Equals(userEmail) || ex.UserEmailId.Equals(userEmail)).ToListAsync();
        }
    }
}
