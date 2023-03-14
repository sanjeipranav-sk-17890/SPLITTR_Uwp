using System.Collections.Generic;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.SplittrNotifications;

namespace SPLITTR_Uwp.Core.DataManager;

public class ExpenseDataManager : IExpenseDataManager
{
    private readonly IExpenseDbHandler _dbHandler;

    public ExpenseDataManager(IExpenseDbHandler dbHandler)
    {
        _dbHandler = dbHandler;
    }
    public Task InsertExpenseAsync(IEnumerable<ExpenseBobj> expenseBobjs)
    {
        return _dbHandler.InsertExpenseAsync(expenseBobjs);
    }
        
    public Task UpdateExpenseAsync(ExpenseBobj expenseBobj)
    {
        return _dbHandler.UpdateExpenseAsync(expenseBobj);
        // SplittrNotification.InvokeExpenseStatusChanged(new ExpenseStatusChangedEventArgs(expenseBobj.ExpenseStatus,expenseBobj));
    }

}