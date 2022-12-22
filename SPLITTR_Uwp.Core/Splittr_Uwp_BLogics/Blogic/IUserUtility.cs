using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic
{
    public interface IUserUtility : IUseCase
    {
        public Task UpdateUserObjAsync(UserBobj userBobj, string newUserName, Currency currencyPreference);
        public Task UpdateUserObjAsync(UserBobj userBobj, double walletBalance);

        //Provides Suggestions Based on User name
        public Task GetUsersSuggestionAsync(string userName,Action<IEnumerable<User>> resultCallBack);
    }
}
