using System;
using System.Collections.Generic;
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
    public class ExpenseDataHandler : IExpenseDataHandler
    {
        private readonly IExpenseDataServices _dataServices;
        private readonly ICurrencyCalcFactory _currencyCalcFactory;
        private  IUserDataHandler _userDataHandler;


        public ExpenseDataHandler(IExpenseDataServices dataServices,ICurrencyCalcFactory currencyCalcFactory)
        {
            _dataServices = dataServices;
            _currencyCalcFactory = currencyCalcFactory;
            
        }

        public Task InsertExpenseAsync(ExpenseBobj expenseBobj)
        {
           return _dataServices.InsertExpenseAsync(expenseBobj);
        }
        
        public Task UpdateExpenseAsync(ExpenseBobj expenseBobj)
        {
            return _dataServices.UpdateExpenseAsync(expenseBobj);
        }

        //public async Task<IEnumerable<ExpenseBobj>> GetUserExpensesAsync(string userEmailId)
        //{
        //    //Fetching Current User's Currency Preference
        //    var userCurrencyPreference =(Currency)( await _userDataHandler.FetchCurrentUserDetails(userEmailId).ConfigureAwait(false)).CurrencyIndex;

        //    //getting ICurrencyConverter Based on Currency PReference
        //    var currencyCalculator = _currencyCalcFactory.GetCurrencyCalculator(userCurrencyPreference);

        //    //fetching Expense entity obj from db  
        //    var userExpenses = await _dataServices.SelectUserExpensesAsync(userEmailId).ConfigureAwait(false);

        //    var outputList = new List<ExpenseBobj>();
        //    Parallel.ForEach(userExpenses, (async expense =>
        //    {
        //        var user =await _userDataHandler.FetchCurrentUserDetails(expense.UserEmailId).ConfigureAwait(false);
        //        outputList.Add(new ExpenseBobj(user,this,currencyConverter: currencyCalculator,expense: expense)); 
        //    }));
        //    return outputList;
        //}
        public async Task<IEnumerable<ExpenseBobj>> GetUserExpensesAsync(User  user, IUserDataHandler userDataHandler)
        {

            _userDataHandler = userDataHandler;

            //Fetching Current User's Currency Preference
            var userCurrencyPreference =(Currency)user.CurrencyIndex;
                /*(Currency)(await _userDataHandler.FetchCurrentUserDetails(userEmailId).ConfigureAwait(false)).CurrencyIndex;*/
            //getting ICurrencyConverter Based on Currency PReference
            var currencyCalculator = _currencyCalcFactory.GetCurrencyCalculator(userCurrencyPreference);

            //fetching Expense entity obj from db  
            var userExpenses = await _dataServices.SelectUserExpensesAsync(user.EmailId).ConfigureAwait(false);

            var outputList = new List<ExpenseBobj>();
            Parallel.ForEach(userExpenses, (async expense =>
            {
                User respectiveUserObj;

                if (expense.UserEmailId == user.EmailId)
                {
                    respectiveUserObj = user;
                }
                else
                {
                  respectiveUserObj = await _userDataHandler.FetchUserUsingMailId(expense.UserEmailId).ConfigureAwait(false);
                }
                outputList.Add(new ExpenseBobj(respectiveUserObj, this, currencyConverter: currencyCalculator, expense: expense));
            }));
            return outputList;
        }
    }
}
