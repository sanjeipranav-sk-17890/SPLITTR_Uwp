using System.Threading;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.UseCase.UpdateUser;

namespace SPLITTR_Uwp.Core.UseCase.AddWalletAmount;

public class AddWalletAmountRequestObject : IRequestObj<UpdateUserResponseObj>
{
    public AddWalletAmountRequestObject(IPresenterCallBack<UpdateUserResponseObj> presenterCallBack, CancellationToken cts, UserBobj currentUser, double walletBalance)
    {
        PresenterCallBack = presenterCallBack;
        Cts = cts;
        CurrentUser = currentUser;
        WalletBalance = walletBalance;
    }

    public CancellationToken Cts { get; }

    public IPresenterCallBack<UpdateUserResponseObj> PresenterCallBack { get; }

    public UserBobj CurrentUser { get; }

    public double WalletBalance { get; }

}
