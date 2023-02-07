using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager.Contracts;

namespace SPLITTR_Uwp.Core.UseCase.SignUpUser
{
    public class SignUpUser : UseCaseBase<SignUpUserResponseObj>
    {
        private readonly SignUpUserReqObj _requestObj;
        private readonly ISignUpDataManager _dataManager;
        public SignUpUser(SignUpUserReqObj requestObj,ISignUpDataManager dataManager) : base(requestObj.PresenterCallBack,requestObj.Cts)
        {
            _requestObj = requestObj;
            _dataManager = dataManager;
        }
        public override void Action()
        {
         _dataManager.CreateNewUser(_requestObj.UserName,_requestObj.EmailId,_requestObj.CurrencyPreference,new UseCaseCallBackBase<SignUpUserResponseObj>(this));  
        }
    }
}
