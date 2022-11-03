using SPLITTR_Uwp.Core.ModelBobj;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DataHandler.Contracts
{
    public interface IUserDataHandler
    {
        public Task<UserBobj> FetchCurrentUserDetails(string emailId);
        public Task<bool> IsUserAlreadyExist(string emailId);
        public Task<int> CreateNewUser(string userName, string emailId,int currencyPreference);
        public Task UpdateUserBobjAsync (UserBobj user);
        public Task<User> FetchUserUsingMailId(string mailId);
        public Task<IEnumerable<User>> GetUsersSuggestionAsync(string userName);
    }
}
