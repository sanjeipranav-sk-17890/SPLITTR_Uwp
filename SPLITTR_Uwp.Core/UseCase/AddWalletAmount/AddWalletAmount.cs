using System;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.UseCase.UpdateUser;

namespace SPLITTR_Uwp.Core.UseCase.AddWalletAmount;

public class AddWalletAmount : UseCaseBase<UpdateUserResponseObj>
{
    private readonly AddWalletAmountRequestObject _requestObj;

    private readonly IAddWalletBalanceDataManager _dataManager;

    public AddWalletAmount(AddWalletAmountRequestObject requestObject, IAddWalletBalanceDataManager dataManager) : base(requestObject.PresenterCallBack, requestObject.Cts)
    {
        _requestObj = requestObject;
        _dataManager = dataManager;
    }
    public override void Action()
    {
        _dataManager.UpdateUserObjAsync(_requestObj.CurrentUser, _requestObj.WalletBalance,new UseCaseCallBackBase<UpdateUserResponseObj>(this));
    }
}
