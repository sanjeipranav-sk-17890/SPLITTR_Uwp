using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SPLITTR_Uwp.Core.UseCase.LoginUser
{
    public class UserLogin : UseCaseBase<LoginResponseObj>
    {
        private readonly LoginRequestObj _requestObj;
        private readonly IAuthenticationManager _dataManager;
        public UserLogin(LoginRequestObj requestObj,IAuthenticationManager dataManager) : base(requestObj.PresenterCallBack,requestObj.Cts)
        {
            _requestObj = requestObj;
            _dataManager = dataManager;
        }
        public override void Action()
        {
            _dataManager.Authenticate(_requestObj.UserEmailId,new UseCaseCallBackBase<LoginResponseObj>(this));
        }
    }
    public interface IAuthenticationManager
    {
        void Authenticate(string emailId, IUseCaseCallBack<LoginResponseObj> callBack);
    }

}
