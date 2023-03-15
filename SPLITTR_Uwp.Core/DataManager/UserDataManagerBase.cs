using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrEventArgs;
using SPLITTR_Uwp.Core.SplittrNotifications;

namespace SPLITTR_Uwp.Core.DataManager;

public class UserDataManagerBase : IUserDataManager
{
    private readonly IUserDbHandler _userDbHandler;
    private readonly ICurrencyCalcFactory _currencyCalc;
    private string _currentUserEmailId;
    private User _currentUser;
    private readonly ConcurrentDictionary<string, User> _localUserCache = new ConcurrentDictionary<string, User>();

    public UserDataManagerBase(IUserDbHandler userDbHandler,ICurrencyCalcFactory currencyCalc)
    {
        _userDbHandler = userDbHandler;
        _currencyCalc = currencyCalc;
    }

    public async Task UpdateUserBobjAsync(User user)
    {
        //if updating User is Current User Then Changing Local reference of that Object
        if (user.Equals(_currentUser))
        {
            UpdateLocalUserCacheData(_currentUser, user);
        }
        //updating Changes in local Cache Collection 
        if (!_localUserCache.ContainsKey(user.EmailId))
        {
            await  _userDbHandler.UpDateUserAsync(user).ConfigureAwait(false); 
            InvokeNotification(user); 
            return;
        }
        var cacheUser = _localUserCache[user.EmailId];
        UpdateLocalUserCacheData(cacheUser, user);


        await _userDbHandler.UpDateUserAsync(user).ConfigureAwait(false);

        //Invoking Notification User Updated
        InvokeNotification(user);

        void InvokeNotification(User user1)
        {
            if (user is UserBobj currentUser)
            {
                SplittrNotification.InvokeUserObjUpdated(new UserBobjUpdatedEventArgs(currentUser));
            }
        }
    }

    private void UpdateLocalUserCacheData(User oldUserData, User newUserData)
    {
        oldUserData.UserName = newUserData.UserName;
        oldUserData.CurrencyIndex = newUserData.CurrencyIndex;
        oldUserData.WalletBalance = newUserData.WalletBalance;
        oldUserData.LentAmount = newUserData.LentAmount;
        oldUserData.OwingAmount = newUserData.OwingAmount;
    }
    public async Task<User> FetchUserUsingMailId(string mailId)
    {
        if (mailId.Equals(_currentUserEmailId))
        {
            if (_currentUser is null || !_currentUser.EmailId.Equals(_currentUserEmailId))
            {
                _currentUser = await _userDbHandler.SelectUserObjByEmailId(mailId).ConfigureAwait(false);
            }
            return _currentUser;
        }
            
        if (!_localUserCache.ContainsKey(mailId)) 
        {
            var user =await _userDbHandler.SelectUserObjByEmailId(mailId).ConfigureAwait(false);
            return  _localUserCache.GetOrAdd(mailId, user);
        } 
            
        return _localUserCache[mailId];
            
    }


    /// <summary>
    /// Fetches List Of Users From Db Excluding Current User
    /// </summary>
    /// <returns></returns>

    public async Task<IEnumerable<User>> GetUsersSuggestionAsync(string userName)
    {
        var usersList =await _userDbHandler.SelectUserFormUsers(userName.Trim()).ConfigureAwait(false);
        return usersList.Where(user => user.EmailId != _currentUserEmailId).ToList();
    }

    public async Task<UserBobj> FetchCurrentUserDetails(string emailId)
    {
        _currentUserEmailId = emailId ?? throw new ArgumentNullException(nameof(emailId),"Passed Email Id Must be a Validated Mail Id");

        var user = await FetchUserUsingMailId(emailId).ConfigureAwait(false);

        var currencyCal = _currencyCalc.GetCurrencyCalculator((Currency)user.CurrencyIndex);

        return new UserBobj(user,currencyCal);

    }
}