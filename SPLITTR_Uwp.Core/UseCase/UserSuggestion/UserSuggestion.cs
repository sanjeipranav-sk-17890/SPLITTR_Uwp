using System.Collections.Generic;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.DependencyInjector;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.UseCase.UserSuggestion
{

    public class UserSuggestionRequestObject : IRequestObj<UserSuggestionResponseObject>
    {
        public UserSuggestionRequestObject(IPresenterCallBack<UserSuggestionResponseObject> presenterCallBack, CancellationToken cts, string userName)
        {
            PresenterCallBack = presenterCallBack;
            Cts = cts;
            UserName = userName;
        }

        public CancellationToken Cts { get; }

        public IPresenterCallBack<UserSuggestionResponseObject> PresenterCallBack { get; }

        public string UserName { get; }
    }
    public class UserSuggestionResponseObject
    {
        public UserSuggestionResponseObject(IEnumerable<User> userSuggestions)
        {
            UserSuggestions = userSuggestions;
        }

        public IEnumerable<User> UserSuggestions { get; }
    }
    public class UserSuggestion : UseCaseBase<UserSuggestionResponseObject>,IUseCaseCallBack<UserSuggestionResponseObject>
    {
        private readonly UserSuggestionRequestObject _requestObj;

        private readonly IUserSuggestionDataManager _dataManager;
        public UserSuggestion( UserSuggestionRequestObject requestObj) : base(requestObj.PresenterCallBack,requestObj.Cts)
        {
            _requestObj = requestObj;
            _dataManager = SplittrDependencyService.GetInstance<IUserSuggestionDataManager>();
        }
       protected override void Action()
        {
          _dataManager.GetUsersSuggestionAsync(_requestObj.UserName,this);   
        }
        void IUseCaseCallBack<UserSuggestionResponseObject>.OnSuccess(UserSuggestionResponseObject result)
        {
            PresenterCallBack?.OnSuccess(result);
        }
        void IUseCaseCallBack<UserSuggestionResponseObject>.OnError(SplittrException ex)
        {
            PresenterCallBack?.OnError(ex);
        }
    }
}
