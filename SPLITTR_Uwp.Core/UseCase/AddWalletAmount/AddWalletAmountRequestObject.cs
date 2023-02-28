using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.UseCase.UpdateUser;

namespace SPLITTR_Uwp.Core.UseCase.AddWalletAmount;

public class AddWalletAmountRequestObject : SplittrRequestBase<UpdateUserResponseObj>
{
    public AddWalletAmountRequestObject(IPresenterCallBack<UpdateUserResponseObj> presenterCallBack, CancellationToken cts, UserBobj currentUser, double walletBalance) : base(cts,presenterCallBack)
    {
        CurrentUser = currentUser;
        WalletBalance = walletBalance;
    }


    public UserBobj CurrentUser { get; }

    public double WalletBalance { get; }

}
