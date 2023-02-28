using System.Threading;

namespace SPLITTR_Uwp.Core.UseCase.LoginUser;

public class LoginRequestObj :SplittrRequestBase<LoginResponseObj>
{
    public LoginRequestObj(string userEmailId, IPresenterCallBack<LoginResponseObj> presenterCallBack, CancellationToken cts) : base(cts,presenterCallBack)
    {
        UserEmailId = userEmailId;
    }

    public string UserEmailId { get;}
}