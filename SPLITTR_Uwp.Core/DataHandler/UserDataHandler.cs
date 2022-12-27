using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.CurrencyCoverter.Factory;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Services.Contracts;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics;

namespace SPLITTR_Uwp.Core.DataHandler
{
    public class UserDataHandler : IUserDataHandler
    { 
        readonly IUserDataServices _userDataServices;
        readonly IUserBobjBalanceCalculator _balanceCalculator;
        readonly IGroupDataHandler _groupDataHandler;
        readonly IExpenseDataHandler _expenseDataHandler;
        private readonly ICurrencyCalcFactory _currencyCalc;
        private string _currentUserEmailId;
        private User _currentUser;
        private readonly ConcurrentDictionary<string, User> _localUserCache = new ConcurrentDictionary<string, User>();

        public UserDataHandler(IUserDataServices userDataServices,IUserBobjBalanceCalculator balanceCalculator, IGroupDataHandler groupDataHandler, IExpenseDataHandler expenseDataHandler,ICurrencyCalcFactory currencyCalc)
        {
            _userDataServices = userDataServices;
            _balanceCalculator = balanceCalculator;
            _groupDataHandler = groupDataHandler;
            _expenseDataHandler = expenseDataHandler;
            _currencyCalc = currencyCalc;
        }

        public Task<int> SignUpUserAsync(User user)
        {
            return _userDataServices.InsertUserObjAsync(user);
        }

        public async Task<bool> IsUserAlreadyExist(string emailId)
        {
           return await Task.Run(async () =>
           {
               var userObj = await _userDataServices.SelectUserObjByEmailId(emailId).ConfigureAwait(false);

               return userObj is not null;
           }).ConfigureAwait(false);
           
        }

        public Task<int> CreateNewUser(string userName, string emailId, int currencyPreference)
        {
            var newUser = new User(emailId, userName, 0.0,currencyPreference);
            return _userDataServices.InsertUserObjAsync(newUser);
        }


        public Task UpdateUserBobjAsync(UserBobj user)
        {           
            return _userDataServices.UpDateUserAsync(user);
        }
        public async Task<User> FetchUserUsingMailId(string mailId)
        {
            if (mailId.Equals(_currentUserEmailId))
            {
                if (_currentUser is null || !_currentUser.EmailId.Equals(_currentUserEmailId))
                {
                    _currentUser = await _userDataServices.SelectUserObjByEmailId(mailId).ConfigureAwait(false);
                }
                return _currentUser;
            }
            
            if (!_localUserCache.ContainsKey(mailId)) 
            {
                var user =await _userDataServices.SelectUserObjByEmailId(mailId).ConfigureAwait(false);
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
            var usersList =await _userDataServices.SelectUserFormUsers(userName.Trim()).ConfigureAwait(false);
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

            //Passing In userDataHandler as Method injection to Avoid Circular Dependency in IServiceCollection 
            var expenses =await _expenseDataHandler.GetUserExpensesAsync(_currentUser,this).ConfigureAwait(false);

            var groups =await _groupDataHandler.GetUserPartcipatingGroups(_currentUser,this).ConfigureAwait(false);

            var currencyCal = _currencyCalc.GetCurrencyCalculator((Currency)user.CurrencyIndex);


            return new UserBobj(user, expenses.ToList(), groups, _balanceCalculator,currencyCal);

        }
    }

}
