using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic
{
    public interface IUserUtility
    {
        public Task UpdateUserObjAsync(UserBobj userBobj, string newUserName, Currency currencyPreference);
        public Task UpdateUserObjAsync(UserBobj userBobj, double walletBalance);
    }
}
