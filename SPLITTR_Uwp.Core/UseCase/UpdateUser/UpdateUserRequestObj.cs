using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;

namespace SPLITTR_Uwp.Core.UseCase.UpdateUser
{
    public class UpdateUserRequestObj :IRequestObj<UpdateUserResponseObj>
    {
        public UpdateUserRequestObj(CancellationToken cts, IPresenterCallBack<UpdateUserResponseObj> presenterCallBack, UserBobj currentUser, string newUserName, Currency currencyPreference)
        {
            Cts = cts;
            PresenterCallBack = presenterCallBack;
            CurrentUser = currentUser;
            NewUserName = newUserName;
            CurrencyPreference = currencyPreference;
        }

        public CancellationToken Cts { get; }

        public IPresenterCallBack<UpdateUserResponseObj> PresenterCallBack { get; }

        public UserBobj CurrentUser { get; }

        public string NewUserName { get; }

        public Currency CurrencyPreference { get; }


    }
    

}
