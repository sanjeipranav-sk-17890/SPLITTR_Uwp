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
            
            var userObj = await  _userDataServices.SelectUserObjByEmailId(emailId).ConfigureAwait(false);

            return userObj is not null;

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
            if (mailId != null && mailId.Equals(_currentUserEmailId))
            {
                return _currentUser ??= await _userDataServices.SelectUserObjByEmailId(mailId).ConfigureAwait(false);
            }
            return await _userDataServices.SelectUserObjByEmailId(mailId).ConfigureAwait(false);
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

            var expenses =await _expenseDataHandler.GetUserExpensesAsync(_currentUser).ConfigureAwait(false);

            var groups =await _groupDataHandler.GetUserPartcipatingGroups(_currentUser).ConfigureAwait(false);

            var currencyCal = _currencyCalc.GetCurrencyCalculator((Currency)user.CurrencyIndex);


            return new UserBobj(user, expenses.ToList(), groups, _balanceCalculator,currencyCal);

        }
    }

}
