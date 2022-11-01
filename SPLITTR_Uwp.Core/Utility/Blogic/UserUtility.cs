using System;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;

namespace SPLITTR_Uwp.Core.Utility.Blogic;

public class UserUtility :IUserUtility
{

    readonly IUserDataHandler _userDataHandler;
    private readonly ICurrencyCalcFactory _factory;
    public UserUtility(IUserDataHandler userDataHandler,ICurrencyCalcFactory factory)
    {
        _userDataHandler = userDataHandler;
        _factory = factory;
    }


    public Task UpdateUserObjAsync(UserBobj userBobj, string newUserName, Currency currencyPreference)
    {
       
            //no db call db call is made if previous value and current value are same
            if (userBobj.UserName.Equals(newUserName.Trim()) && userBobj.CurrencyPreference == currencyPreference)
            {
                return Task.CompletedTask;
            }

            userBobj.UserName = newUserName;

            //based on newly selected currency Preference feching the ICurrency converter
            ICurrencyConverter currencyConverter = _factory.GetCurrencyCalculator(currencyPreference);

            //changing each currency converter in Expense so setting and fetching value can be made in corresponing currency formats
            foreach (var expense in userBobj.Expenses)
            {
                expense.CurrencyConverter = currencyConverter;
            }

            //updating IConverter so fetching wallet  amount will be fetched in requered currency preference 
            userBobj.CurrencyConverter = currencyConverter;
            userBobj.CurrencyPreference = currencyPreference;


           return _userDataHandler.UpdateUserBobjAsync(userBobj);
        
    }
    public Task UpdateUserObjAsync(UserBobj userBobj, double walletBalance)
    {
       
            userBobj.WalletBalance += walletBalance;
            return _userDataHandler.UpdateUserBobjAsync(userBobj);
            
    }
}
