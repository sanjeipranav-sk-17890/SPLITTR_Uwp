using System;
using System.Collections.Concurrent;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DataManager.ServiceObjects;
using SPLITTR_Uwp.Core.DbHandler.SqliteConnection;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;

namespace SPLITTR_Uwp.Core.DataManager
{
    public class ExpenseHistoryManager :UseCaseBase, IExpenseHistoryManager,IExpenseHistoryUsecase
    {
        private readonly ISqlDataServices _sqlDataServices;

        private readonly ConcurrentDictionary<string, ExpenseHistory> _expenseHistory = new ConcurrentDictionary<string, ExpenseHistory>(); 

        public ExpenseHistoryManager(ISqlDataServices sqlDataServices)
        {
            _sqlDataServices = sqlDataServices;
            _sqlDataServices.CreateTable<ExpenseHistory>();

        }
        public void RecordExpenseMarkedAsPaid(string expenseId)
        {
             RunAsynchronously(() =>
            {
                var recordExpenseHistory = new ExpenseHistory(expenseId);

                 _sqlDataServices.InsertObj(recordExpenseHistory);

            });
        }
        private object _lock = new object();
        public  void IsExpenseMarkedAsPaid(string expenseId, Action<bool> resultCallBack)
        {
            RunAsynchronously(async () =>
            {
                lock (_lock)
                {
                    var isExpenseMarkedAsPaid = _expenseHistory.ContainsKey(expenseId);

                    if (isExpenseMarkedAsPaid)
                    {
                        resultCallBack?.Invoke(true);
                        return;
                    }

                }
                
                var expenseHistory = await _sqlDataServices.FetchTable<ExpenseHistory>().FirstOrDefaultAsync(h => h.ExpenseUniqueId.Equals(expenseId)).ConfigureAwait(false);

                switch (expenseHistory)
                {
                    //if null(no history record) then that expense is not marked as Paid 
                    case null:
                        resultCallBack?.Invoke(false);
                        return;
                      
                     default:
                        var result= _expenseHistory.GetOrAdd(expenseId, expenseHistory) is null;
                        resultCallBack?.Invoke(result);
                        break;

                }
                
            });
            
        }

    }
}
