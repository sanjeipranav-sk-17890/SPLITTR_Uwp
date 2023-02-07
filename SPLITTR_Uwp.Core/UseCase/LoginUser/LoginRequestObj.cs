using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SPLITTR_Uwp.Core.UseCase.LoginUser
{
    public class LoginRequestObj :IRequestObj<LoginResponseObj>
    {
        public LoginRequestObj(string userEmailId, IPresenterCallBack<LoginResponseObj> presenterCallBack, CancellationToken cts)
        {
            UserEmailId = userEmailId;
            PresenterCallBack = presenterCallBack;
            Cts = cts;
        }

        public CancellationToken Cts { get; }

        public IPresenterCallBack<LoginResponseObj> PresenterCallBack { get; }

        public string UserEmailId { get;}
    }
}
