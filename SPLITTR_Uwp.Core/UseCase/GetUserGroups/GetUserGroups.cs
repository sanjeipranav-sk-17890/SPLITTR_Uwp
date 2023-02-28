using System.Collections.Generic;
using System.Threading;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.UseCase.GetUserGroups
{
    public class GetUserGroups :UseCaseBase<GetUserGroupResponse>
    {
        private readonly GetUserGroupReq _requestObj;

        private readonly IGroupDataManager _groupDataManager;

        public GetUserGroups(GetUserGroupReq requestObj) : base(requestObj.PresenterCallBack,requestObj.Cts)
        {
            _requestObj = requestObj;
            _groupDataManager = SplittrDependencyService.GetInstance<IGroupDataManager>();
        }
       protected override void Action()
        {
            _groupDataManager.GetUserParticipatingGroups(_requestObj.CurrentUser,new GetUserGroupUcCallBack(this));
        }

        private class GetUserGroupUcCallBack : IUseCaseCallBack<GetUserGroupResponse>
        {
            private readonly GetUserGroups _useCase;
            public GetUserGroupUcCallBack(GetUserGroups useCase)
            {
                _useCase = useCase;
            }

            public void OnSuccess(GetUserGroupResponse result)
            {
               _useCase.PresenterCallBack?.OnSuccess(result);
            }
            public void OnError(SplittrException ex)
            {
                _useCase.PresenterCallBack?.OnError(ex);
            }
        }
    }

    public class GetUserGroupReq :IRequestObj<GetUserGroupResponse>
    {
        public GetUserGroupReq(CancellationToken cts, IPresenterCallBack<GetUserGroupResponse> presenterCallBack, User currentUser)
        {
            Cts = cts;
            PresenterCallBack = presenterCallBack;
            CurrentUser = currentUser;
        }

        public CancellationToken Cts { get; }

        public IPresenterCallBack<GetUserGroupResponse> PresenterCallBack { get; }

        public User CurrentUser { get;}


    }
    public class GetUserGroupResponse
    {
        public GetUserGroupResponse(IEnumerable<GroupBobj> userParticipatingGroups)
        {
            UserParticipatingGroups = userParticipatingGroups;
        }

        public IEnumerable<GroupBobj> UserParticipatingGroups { get;  }
    }

}
