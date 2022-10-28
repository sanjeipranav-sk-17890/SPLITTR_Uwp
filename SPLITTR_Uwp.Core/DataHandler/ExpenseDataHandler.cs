using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Services.Contracts;

namespace SPLITTR_Uwp.Core.DataHandler
{
    public class ExpenseDataHandler : IExpenseDataHandler
    {
        private readonly IExpenseDataServices _dataServices;
        private readonly ICurrencyCalcFactory _currencyCalcFactory;
        private readonly IUserDataServices _userDataServices;

        public ExpenseDataHandler(IExpenseDataServices dataServices,ICurrencyCalcFactory currencyCalcFactory,IUserDataServices userDataServices)
        {
            _dataServices = dataServices;
            _currencyCalcFactory = currencyCalcFactory;
            this._userDataServices = userDataServices;
        }

        public Task InsertExpenseAsync(ExpenseBobj expenseBobj)
        {
           return _dataServices.InsertExpenseAsync(expenseBobj);
        }
        
        public Task UpdateExpenseAsync(ExpenseBobj expenseBobj)
        {
            return _dataServices.UpdateExpenseAsync(expenseBobj);
        }

        public async Task<IEnumerable<ExpenseBobj>> GetUserExpensesAsync(string userEmailId)
        {
            //Fetching Current User's Currency Preference
            var userCurrencyPreference =(Currency)( await _userDataServices.SelectUserObjByEmailId(userEmailId)).CurrencyIndex;

            //getting ICurrencyConverter Based on Currency PReference
            var currencyCalculator = _currencyCalcFactory.GetCurrencyCalculator(userCurrencyPreference);

            //fetching Expense entity obj from db  
            var userExpenses = await _dataServices.SelectUserExpensesAsync(userEmailId);

            var outputList = new List<ExpenseBobj>();
            Parallel.ForEach(userExpenses, (async expense =>
            {
                var user =await _userDataServices.SelectUserObjByEmailId(expense.UserEmailId).ConfigureAwait(false);
                outputList.Add(new ExpenseBobj(user,this,currencyConverter: currencyCalculator,expense: expense)); 
            }));
            return outputList;
        }
    }
}
