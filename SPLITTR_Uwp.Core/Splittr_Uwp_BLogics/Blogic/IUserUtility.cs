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
        public void UpdateUserObjAsync(UserBobj userBobj, string newUserName, Currency currencyPreference,Action onSuccessCallBack);
        public void UpdateUserObjAsync(UserBobj userBobj, double walletBalance, Action onSuccessCallBack);

        //Provides Suggestions Based on User name
        public void GetUsersSuggestionAsync(string userName,Action<IEnumerable<User>> resultCallBack);
    }
}
