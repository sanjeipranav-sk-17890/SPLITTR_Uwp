using System.Collections.Generic;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DataManager.Contracts
{
    public interface IUserDataManager
    {
        public Task<UserBobj> FetchCurrentUserDetails(string emailId);
        public Task<bool> IsUserAlreadyExist(string emailId);
        public Task<int> CreateNewUser(string userName, string emailId,int currencyPreference);
        public Task UpdateUserBobjAsync (User user);
        public Task<User> FetchUserUsingMailId(string mailId);
        public Task<IEnumerable<User>> GetUsersSuggestionAsync(string userName);
    }
}
