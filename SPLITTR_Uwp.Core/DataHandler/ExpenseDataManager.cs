using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Services.Contracts;

namespace SPLITTR_Uwp.Core.DataHandler
{
    public class ExpenseDataManager : IExpenseDataHandler
    {
        private readonly IExpenseDataServices _dataServices;
        private readonly ICurrencyCalcFactory _currencyCalcFactory;
        private  IUserDataHandler _userDataHandler;


        public ExpenseDataManager(IExpenseDataServices dataServices,ICurrencyCalcFactory currencyCalcFactory)
        {
            _dataServices = dataServices;
            _currencyCalcFactory = currencyCalcFactory;
            
        }

        public Task InsertExpenseAsync(IEnumerable<ExpenseBobj> expenseBobjs)
        {
           return _dataServices.InsertExpenseAsync(expenseBobjs);
        }
        
        public Task UpdateExpenseAsync(ExpenseBobj expenseBobj)
        {
            return _dataServices.UpdateExpenseAsync(expenseBobj);
        }

        public async Task<IEnumerable<ExpenseBobj>> GetUserExpensesAsync(User  user, IUserDataHandler userDataHandler)
        {

            _userDataHandler = userDataHandler;
            //fetching Expense entity obj from db  
            var userExpenses = await _dataServices.SelectUserExpensesAsync(user.EmailId).ConfigureAwait(false);

            return await InitializeExpenseBobjs(userExpenses, user).ConfigureAwait(false);
        }
        public async Task<IEnumerable<ExpenseBobj>> GetRelatedExpenses(ExpenseBobj expenseBobj, User currentUser)
        {
            var key = expenseBobj.ParentExpenseId ?? expenseBobj.ExpenseUniqueId;

            var relatedExpenseList =await _dataServices.SelectRelatedExpenses(key).ConfigureAwait(false);

            return await InitializeExpenseBobjs(relatedExpenseList, currentUser).ConfigureAwait(false);
        }


        async Task<IEnumerable<ExpenseBobj>> InitializeExpenseBobjs(IEnumerable<Expense> expenses,User currentUser)
        {
            //Fetching Current User's Currency Preference
            var userCurrencyPreference = (Currency)currentUser.CurrencyIndex;


            //getting ICurrencyConverter Based on Currency PReference
            var currencyCalculator = _currencyCalcFactory.GetCurrencyCalculator(userCurrencyPreference);


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


                requestedOwnerUserObj ??= await _userDataHandler.FetchUserUsingMailId(expense.RequestedOwner).ConfigureAwait(false);

                respectiveUserObj ??= await _userDataHandler.FetchUserUsingMailId(expense.UserEmailId).ConfigureAwait(false);


                return new ExpenseBobj(respectiveUserObj, requestedOwnerUserObj, currencyConverter: currencyCalculator, expense: expense);
            }

        }



    }
}
