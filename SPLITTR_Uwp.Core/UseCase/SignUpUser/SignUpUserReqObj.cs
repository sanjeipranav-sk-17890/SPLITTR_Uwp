using System.Threading;

namespace SPLITTR_Uwp.Core.UseCase.SignUpUser
{
    public class SignUpUserReqObj : IRequestObj<SignUpUserResponseObj>
    {
        public SignUpUserReqObj(int currencyPreference, string emailId, string userName, IPresenterCallBack<SignUpUserResponseObj> presenterCallBack, CancellationToken cts)
        {
            CurrencyPreference = currencyPreference;
            EmailId = emailId;
            UserName = userName;
            PresenterCallBack = presenterCallBack;
            Cts = cts;
        }

        public CancellationToken Cts { get; }

        public IPresenterCallBack<SignUpUserResponseObj> PresenterCallBack { get; }

        public string UserName { get;}

        public string EmailId { get;}

        public int CurrencyPreference { get;}

    }
}
