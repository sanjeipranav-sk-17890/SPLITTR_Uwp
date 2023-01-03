using System;
using System.Collections.Generic;
using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;

public class UserUtility : UseCaseBase, IUserUtility
{

    readonly IUserDataHandler _userDataHandler;
    private readonly ICurrencyCalcFactory _factory;
    public UserUtility(IUserDataHandler userDataHandler, ICurrencyCalcFactory factory)
    {
        _userDataHandler = userDataHandler;
        _factory = factory;
    }


    public async Task UpdateUserObjAsync(UserBobj userBobj, string newUserName, Currency currencyPreference)
    {
        await RunAsynchronously(async () =>
        {
            //no db call db call is made if previous value and current value are same
            if (userBobj.UserName.Equals(newUserName.Trim()) && userBobj.CurrencyPreference == currencyPreference)
            {
                return;
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

            //since it is updating on db in background We are waiting until it is completed
            await _userDataHandler.UpdateUserBobjAsync(userBobj).ConfigureAwait(false);

        }).ConfigureAwait(false);

    }
    public Task UpdateUserObjAsync(UserBobj userBobj, double walletBalance)
    {
        return RunAsynchronously(async () =>
         {
             userBobj.StrWalletBalance += walletBalance;
             await _userDataHandler.UpdateUserBobjAsync(userBobj).ConfigureAwait(false);

         });

    }
    public Task GetUsersSuggestionAsync(string userName, Action<IEnumerable<User>> resultCallBack)
    {
        return RunAsynchronously(async () =>
        {
           var suggestionList = await _userDataHandler.GetUsersSuggestionAsync(userName).ConfigureAwait(false);

          resultCallBack?.Invoke(suggestionList);
        } );
    }

}
