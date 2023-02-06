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

namespace SPLITTR_Uwp.Core.DataManager
{
    public class UserDataManager : IUserDataManager
    {
        private readonly IUserDbHandler _userDbHandler;
        private readonly IGroupDataManager _groupDataManager;
        private readonly IExpenseDataManager _expenseDataManager;
        private readonly ICurrencyCalcFactory _currencyCalc;
        private string _currentUserEmailId;
        private User _currentUser;
        private readonly ConcurrentDictionary<string, User> _localUserCache = new ConcurrentDictionary<string, User>();

        public UserDataManager(IUserDbHandler userDbHandler, IGroupDataManager groupDataManager, IExpenseDataManager expenseDataManager,ICurrencyCalcFactory currencyCalc)
        {
            _userDbHandler = userDbHandler;
            _groupDataManager = groupDataManager;
            _expenseDataManager = expenseDataManager;
            _currencyCalc = currencyCalc;
        }

        public Task<int> SignUpUserAsync(User user)
        {
            return _userDbHandler.InsertUserObjAsync(user);
        }

        public async Task<bool> IsUserAlreadyExist(string emailId)
        {
           return await Task.Run(async () =>
           {
               var userObj = await _userDbHandler.SelectUserObjByEmailId(emailId).ConfigureAwait(false);

               return userObj is not null;
           }).ConfigureAwait(false);
           
        }

        public Task<int> CreateNewUser(string userName, string emailId, int currencyPreference)
        {
            var newUser = new User(emailId, userName, 0.0,currencyPreference,0.0,0.0);
            return _userDbHandler.InsertUserObjAsync(newUser);
        }


        public Task UpdateUserBobjAsync(User user)
        {
            //updating Changes in local Cache Collection 
            if (!_localUserCache.ContainsKey(user.EmailId))
            {
                //if updating User is Current User Then Changing Local reference of that Object
                if (user.Equals(_currentUser))
                {
                    UpdateLocalUserCacheData(_currentUser, user);
                }
                return _userDbHandler.UpDateUserAsync(user);
            }
            var cacheUser = _localUserCache[user.EmailId];
            UpdateLocalUserCacheData(cacheUser, user);

            return _userDbHandler.UpDateUserAsync(user);
        }

        void UpdateLocalUserCacheData(User oldUserData, User newUserData)
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
            var outputList = new List<User>();
            foreach (var user in usersList)
            {
                if (user.EmailId != _currentUserEmailId)
                {
                     outputList.Add(user);
                }
            }
            return outputList;
        }

        public async Task<UserBobj> FetchCurrentUserDetails(string emailId)
        {
            _currentUserEmailId = emailId ?? throw new ArgumentNullException(nameof(emailId),"Passed Email Id Must be a Validated Mail Id");

            var user = await FetchUserUsingMailId(emailId).ConfigureAwait(false);

            //Passing In userDataManager as Method injection to Avoid Circular Dependency in IServiceCollection 
            var expenses =await _expenseDataManager.GetUserExpensesAsync(_currentUser,this).ConfigureAwait(false);

            var groups =await _groupDataManager.GetUserPartcipatingGroups(_currentUser,this).ConfigureAwait(false);

            var currencyCal = _currencyCalc.GetCurrencyCalculator((Currency)user.CurrencyIndex);


            return new UserBobj(user, expenses.ToList(), groups,currencyCal);

        }
    }

}
