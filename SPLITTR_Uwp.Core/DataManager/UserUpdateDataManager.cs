using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.SplittrEventArgs;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.UpdateUser;
using SPLITTR_Uwp.Core.UseCase.UserSuggestion;
using SQLite;

namespace SPLITTR_Uwp.Core.DataManager;


public interface IUserProfileUpdateDataManager
{
    public void UpdateUserObjAsync(UserBobj userBobj, string newUserName, Currency currencyPreference, IUseCaseCallBack<UpdateUserResponseObj> callBack);
}
public interface IAddWalletBalanceDataManager
{
    public void UpdateUserObjAsync(UserBobj userBobj, double walletBalance, IUseCaseCallBack<UpdateUserResponseObj> callBack);
}

public interface IUserSuggestionDataManager
{
    public void GetUsersSuggestionAsync(string userName, IUseCaseCallBack<UserSuggestionResponseObject> callBack);

}
public interface IUserUpdateDataManager : IUserProfileUpdateDataManager, IAddWalletBalanceDataManager, IUserSuggestionDataManager
{

}

public class UserUpdateDataManager : IUserUpdateDataManager
{

    private readonly IUserDataManager _userDataManager;
    private readonly ICurrencyCalcFactory _factory;
    public UserUpdateDataManager(IUserDataManager userDataManager, ICurrencyCalcFactory factory)
    {
        _userDataManager = userDataManager;
        _factory = factory;
    }


    /// <exception cref="UserNameInvalidException">User Name must Be More Than 3 Characters</exception>
    public async void UpdateUserObjAsync(UserBobj userBobj, string newUserName, Currency currencyPreference, IUseCaseCallBack<UpdateUserResponseObj> callBack)
    {
        try
        {
            if (string.IsNullOrEmpty(newUserName) || newUserName.Length < 3)
            {
                throw new UserNameInvalidException("User Name must Be More Than 3 Characters");
            }

            //no db call db call is made if previous value and current value are same
            if (userBobj.UserName.Equals(newUserName.Trim()) && userBobj.CurrencyPreference == currencyPreference)
            {
                callBack?.OnSuccess(new UpdateUserResponseObj(userBobj));
                return;
            }

            userBobj.UserName = newUserName;

            //based on newly selected currency Preference feching the ICurrency converter
            var currencyConverter = _factory.GetCurrencyCalculator(currencyPreference);

            //changing each currency converter in Expense so setting and fetching value can be made in corresponing currency formats
            ExpenseBobj.CurrencyConverter = currencyConverter;

            //updating IConverter so fetching wallet  amount will be fetched in requered currency preference 
            userBobj.CurrencyConverter = currencyConverter;
            userBobj.CurrencyPreference = currencyPreference;

            await _userDataManager.UpdateUserBobjAsync(userBobj).ConfigureAwait(false);

            callBack?.OnSuccess(new UpdateUserResponseObj(userBobj));

            //Raising Notification for Currency Preference change
            SplittrNotification.InvokePreferenceChanged(new CurrencyPreferenceChangedEventArgs(currencyPreference));
        }
        catch (UserNameInvalidException ex)
        {
            callBack?.OnError(new SplittrException(ex,ex.Message));
        }
        catch (SQLiteException ex)
        {
            callBack?.OnError(new SplittrException(ex, "Db Call Failed"));
        }
        
    }
    public async void UpdateUserObjAsync(UserBobj userBobj, double walletBalance, IUseCaseCallBack<UpdateUserResponseObj> callBack)
    {
        try
        {
            userBobj.StrWalletBalance += walletBalance;
            await _userDataManager.UpdateUserBobjAsync(userBobj).ConfigureAwait(false);
            callBack?.OnSuccess(new UpdateUserResponseObj(userBobj));
        }
        catch (SQLiteException ex)
        {
            callBack?.OnError(new SplittrException(ex, "Db Call Failed"));
        }
    }
    public async void GetUsersSuggestionAsync(string userName, IUseCaseCallBack<UserSuggestionResponseObject> callBack)
    {
        try
        {
            var suggestionList = await _userDataManager.GetUsersSuggestionAsync(userName).ConfigureAwait(false);

            callBack.OnSuccess(new UserSuggestionResponseObject(suggestionList));
        }
        catch (SQLiteException e)
        {
            callBack?.OnError(new SplittrException(e, "Db Fetch Error"));
        }


    }

}
