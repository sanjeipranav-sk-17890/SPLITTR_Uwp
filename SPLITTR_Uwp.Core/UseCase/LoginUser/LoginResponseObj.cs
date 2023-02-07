#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.LoginUser
{
    public class LoginResponseObj
    {
        public LoginResponseObj(UserBobj loginUserCred, bool isUserAlreadyExist)
        {
            LoginUserCred = loginUserCred;
            IsUserAlreadyExist = isUserAlreadyExist;
        }

        public bool IsUserAlreadyExist { get;}

        public UserBobj? LoginUserCred { get;}
    }
}
