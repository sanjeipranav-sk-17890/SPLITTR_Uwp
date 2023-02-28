using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;

namespace SPLITTR_Uwp.Core.UseCase.UpdateUser;

public class UpdateUserRequestObj :SplittrRequestBase<UpdateUserResponseObj>
{
    public UpdateUserRequestObj(CancellationToken cts, IPresenterCallBack<UpdateUserResponseObj> presenterCallBack, UserBobj currentUser, string newUserName, Currency currencyPreference):base(cts, presenterCallBack)
    {
        CurrentUser = currentUser;
        NewUserName = newUserName;
        CurrencyPreference = currencyPreference;
    }

    public UserBobj CurrentUser { get; }

    public string NewUserName { get; }

    public Currency CurrencyPreference { get; }


}