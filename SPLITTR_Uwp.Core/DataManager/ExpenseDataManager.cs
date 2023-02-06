using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DataManager
{
    public class ExpenseDataManager : IExpenseDataManager
    {
        private readonly IExpenseDbHandler _dbHandler;
        private readonly ICurrencyCalcFactory _currencyCalcFactory;
        private  IUserDataManager _userDataManager;


        public ExpenseDataManager(IExpenseDbHandler dbHandler,ICurrencyCalcFactory currencyCalcFactory)
        {
            _dbHandler = dbHandler;
            _currencyCalcFactory = currencyCalcFactory;
            
        }

        public Task InsertExpenseAsync(IEnumerable<ExpenseBobj> expenseBobjs)
        {
           return _dbHandler.InsertExpenseAsync(expenseBobjs);
        }
        
        public Task UpdateExpenseAsync(ExpenseBobj expenseBobj)
        {
            return _dbHandler.UpdateExpenseAsync(expenseBobj);
        }

        public async Task<IEnumerable<ExpenseBobj>> GetUserExpensesAsync(User  user, IUserDataManager userDataManager)
        {

            _userDataManager = userDataManager;
            //fetching Expense entity obj from db  
            var userExpenses = await _dbHandler.SelectUserExpensesAsync(user.EmailId).ConfigureAwait(false);

            return await InitializeExpenseBobjs(userExpenses, user).ConfigureAwait(false);
        }

        async Task<IEnumerable<ExpenseBobj>> InitializeExpenseBobjs(IEnumerable<Expense> expenses,User currentUser)
        {
            //Fetching Current User's Currency Preference
            var userCurrencyPreference = (Currency)currentUser.CurrencyIndex;


            //getting ICurrencyConverter Based on Currency PReference
            ExpenseBobj.CurrencyConverter = _currencyCalcFactory.GetCurrencyCalculator(userCurrencyPreference);


            var outputList = new List<ExpenseBobj>();

            //For Each Expense Constructs ExpenseBobj in Worker Thread 
            var tasksOfExpense = expenses.Select(expense => Task.Run((() => ConstructExpenseBobj(expense))));

            var userExpenseBobjs = await Task.WhenAll(tasksOfExpense).ConfigureAwait(false);

            return userExpenseBobjs;


            //Local Function Which Builds ExpenseBobj 
            async Task<ExpenseBobj> ConstructExpenseBobj(Expense expense)
            {

                User respectiveUserObj = null;
                User requestedOwnerUserObj = null;

                if (expense.UserEmailId == currentUser.EmailId)
                {
                    respectiveUserObj = currentUser;
                }
                if (expense.RequestedOwner == currentUser.EmailId)
                {
                    requestedOwnerUserObj = currentUser;
                }


                requestedOwnerUserObj ??= await _userDataManager.FetchUserUsingMailId(expense.RequestedOwner).ConfigureAwait(false);

                respectiveUserObj ??= await _userDataManager.FetchUserUsingMailId(expense.UserEmailId).ConfigureAwait(false);


                return new ExpenseBobj(respectiveUserObj, requestedOwnerUserObj, expense: expense);
            }

        }



    }
}
