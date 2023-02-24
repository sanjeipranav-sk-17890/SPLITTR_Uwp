using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.UseCase.GetUserGroups;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace SPLITTR_Uwp.Core.UseCase.GetGroupDetails
{
    public class GroupDetailById : UseCaseBase<GroupDetailByIdResponse>
    {
        private readonly GroupDetailByIdRequest _requestObj;
        private readonly IGroupDataManager _groupDataManager;

        private readonly static ConcurrentDictionary<string, GroupBobj> _groupBobjCache = new ConcurrentDictionary<string, GroupBobj>();


        public GroupDetailById(GroupDetailByIdRequest requestObj) : base(requestObj.PresenterCallBack, requestObj.Cts)
        {
            _requestObj = requestObj;
            _groupDataManager = SplittrDependencyService.GetInstance<IGroupDataManager>();
        }

        public override bool GetIfAvailableFromCache()
        {
            if (!_groupBobjCache.ContainsKey(_requestObj.GroupUniqueId))
            {
                return false;
            }
            PresenterCallBack?.OnSuccess(new GroupDetailByIdResponse(_groupBobjCache[_requestObj.GroupUniqueId]));
            return true;
        }

        public override void Action()
        {
            _groupDataManager.GetUserParticipatingGroups(_requestObj.CurrentUser, new GroupDetailUsCallBack(this));
        }

        class GroupDetailUsCallBack : IUseCaseCallBack<GetUserGroupResponse>
        {
            private readonly GroupDetailById _useCase;

            public GroupDetailUsCallBack(GroupDetailById useCase)
            {
                _useCase = useCase;
            }

            public void OnSuccess(GetUserGroupResponse responseObj)
            {
                foreach (var group in responseObj?.UserParticipatingGroups)
                {
                    GroupDetailById._groupBobjCache.AddOrUpdate(group.GroupUniqueId, group, ((s, bobj) => bobj));
                }
                try
                {
                    var requestedGroup = GroupDetailById._groupBobjCache[_useCase._requestObj.GroupUniqueId];
                    _useCase?.PresenterCallBack?.OnSuccess(new GroupDetailByIdResponse(requestedGroup));
                }
                catch (KeyNotFoundException ex)
                {
                    _useCase?.PresenterCallBack?.OnError(new SplittrException(ex, "No Such Group Found With That Key"));
                }

            }
            public void OnError(SplittrException error)
            {
                _useCase?.PresenterCallBack?.OnError(error);
            }
        }

    }

    public class GroupDetailByIdRequest : IRequestObj<GroupDetailByIdResponse>
    {
        public GroupDetailByIdRequest(string groupUniqueId, CancellationToken cts, IPresenterCallBack<GroupDetailByIdResponse> presenterCallBack, User currentUser)
        {
            GroupUniqueId = groupUniqueId;
            Cts = cts;
            PresenterCallBack = presenterCallBack;
            CurrentUser = currentUser;
        }

        public CancellationToken Cts { get; }

        public IPresenterCallBack<GroupDetailByIdResponse> PresenterCallBack { get; }

        public string GroupUniqueId { get; }

        public User CurrentUser { get; }

    }
    public class GroupDetailByIdResponse
    {
        public GroupDetailByIdResponse(GroupBobj requestedGroup)
        {
            RequestedGroup = requestedGroup;
        }

        public GroupBobj RequestedGroup { get; }
    }
}