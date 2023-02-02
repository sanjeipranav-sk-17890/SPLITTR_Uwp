using System;
using System.Collections.Generic;
using SPLITTR_Uwp.Core.CurrencyCoverter;
using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;

public class UserUseCase : UseCaseBase, IUserUseCase
{

    readonly IUserDataManager _userDataManager;
    private readonly ICurrencyCalcFactory _factory;
    public UserUseCase(IUserDataManager userDataManager, ICurrencyCalcFactory factory)
    {
        _userDataManager = userDataManager;
        _factory = factory;
    }


    public  void UpdateUserObjAsync(UserBobj userBobj, string newUserName, Currency currencyPreference, Action onSuccessCallBack)
    {
        RunAsynchronously(async () =>
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
            ExpenseBobj.CurrencyConverter = currencyConverter;
           

            //updating IConverter so fetching wallet  amount will be fetched in requered currency preference 
            userBobj.CurrencyConverter = currencyConverter;
            userBobj.CurrencyPreference = currencyPreference;

            //since it is updating on db in background We are waiting until it is completed
            await _userDataManager.UpdateUserBobjAsync(userBobj).ConfigureAwait(false);

            onSuccessCallBack?.Invoke();
        });

    }
    public void UpdateUserObjAsync(UserBobj userBobj, double walletBalance, Action onSuccessCallBack)
    {
         RunAsynchronously(async () =>
         {
             userBobj.StrWalletBalance += walletBalance;
             await _userDataManager.UpdateUserBobjAsync(userBobj).ConfigureAwait(false);
             onSuccessCallBack?.Invoke();
         });

    }
    public void GetUsersSuggestionAsync(string userName, Action<IEnumerable<User>> resultCallBack)
    {
        RunAsynchronously(async () =>
        {
           var suggestionList = await _userDataManager.GetUsersSuggestionAsync(userName).ConfigureAwait(false);

          resultCallBack?.Invoke(suggestionList);
        } );
    }

}
