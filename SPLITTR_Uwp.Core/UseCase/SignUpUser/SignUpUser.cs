using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;

namespace SPLITTR_Uwp.Core.UseCase.SignUpUser
{
    public class SignUpUser : UseCaseBase<SignUpUserResponseObj>
    {
        private readonly SignUpUserReqObj _requestObj;
        private readonly ISignUpDataManager _dataManager;
        public SignUpUser(SignUpUserReqObj requestObj) : base(requestObj.PresenterCallBack,requestObj.Cts)
        {
            _requestObj = requestObj;
            _dataManager = SplittrDependencyService.GetInstance<ISignUpDataManager>();
        }
        public override void Action()
        {
         _dataManager.CreateNewUser(_requestObj.UserName,_requestObj.EmailId,_requestObj.CurrencyPreference,new UseCaseCallBackBase<SignUpUserResponseObj>(this));  
        }
    }
}
