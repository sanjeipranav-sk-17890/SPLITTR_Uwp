using System.Threading;

namespace SPLITTR_Uwp.Core.UseCase.SignUpUser;

public class SignUpUserReqObj : SplittrRequestBase<SignUpUserResponseObj>
{
    public SignUpUserReqObj(int currencyPreference, string emailId, string userName, IPresenterCallBack<SignUpUserResponseObj> presenterCallBack, CancellationToken cts) : base(cts,presenterCallBack)
    {
        CurrencyPreference = currencyPreference;
        EmailId = emailId;
        UserName = userName;
    }

    public string UserName { get;}

    public string EmailId { get;}

    public int CurrencyPreference { get;}

}