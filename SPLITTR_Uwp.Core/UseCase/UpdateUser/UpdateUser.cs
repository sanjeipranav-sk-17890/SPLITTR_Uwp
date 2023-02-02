using System.Collections.Generic;
using System.Text;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.EventArg;

namespace SPLITTR_Uwp.Core.UseCase.UpdateUser
{
    public class UpdateUser : UseCaseBase<UpdateUserResponseObj>,IUseCaseCallBack<UpdateUserResponseObj>
    {
        private readonly IUserProfileUpdateDataManager _dataManager;

        private readonly UpdateUserRequestObj _requestObj;

        public UpdateUser(IUserProfileUpdateDataManager dataManager, UpdateUserRequestObj requestObj) : base(requestObj.PresenterCallBack,requestObj.Cts)
        {
            _dataManager = dataManager;
            _requestObj = requestObj;
        }
        public override void Action()
        {
            _dataManager.UpdateUserObjAsync(_requestObj.CurrentUser,_requestObj.NewUserName,_requestObj.CurrencyPreference,this);
        }
        void IUseCaseCallBack<UpdateUserResponseObj>.OnSuccess(UpdateUserResponseObj responseObj)
        {
           PresenterCallBack?.OnSuccess(responseObj);
        }
        void IUseCaseCallBack<UpdateUserResponseObj>.OnError(SplittrException error)
        {
            PresenterCallBack?.OnError(error);
        }
    }

}
