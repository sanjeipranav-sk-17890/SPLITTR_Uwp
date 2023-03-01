using System.Collections.Concurrent;
using System.Collections.Generic;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.DependencyInjector;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.UseCase.GetUserGroups;

namespace SPLITTR_Uwp.Core.UseCase.GetGroupDetails;

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

    protected override void Action()
    {
        _groupDataManager.GetUserParticipatingGroups(_requestObj.CurrentUser, new GroupDetailUsCallBack(this));
    }

    private class GroupDetailUsCallBack : IUseCaseCallBack<GetUserGroupResponse>
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
                _groupBobjCache.AddOrUpdate(group.GroupUniqueId, group, (s, bobj) => bobj);
            }
            try
            {
                var requestedGroup = _groupBobjCache[_useCase._requestObj.GroupUniqueId];
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
public interface IGroupDetailManager
{
    public void FetchGroupByGroupId(string groupId,User currentUSer,IUseCaseCallBack<GroupDetailByIdResponse> callBack);
    public bool IsCacheAvailable(string groupId);
}
public class GroupDetailManager : GroupDataManagerBase, IGroupDetailManager
{
    
    private readonly static ConcurrentDictionary<string, GroupBobj> _groupBobjCache = new ConcurrentDictionary<string, GroupBobj>();


    public GroupDetailManager(IGroupDbHandler groupDbHandler,IUserDataManager userDataManager,IGroupToUserDbHandler groupToUserDbHandler):base(groupDbHandler,groupToUserDbHandler ,userDataManager)
    {

    }

    public async void FetchGroupByGroupId(string groupId,  User currentUSer,IUseCaseCallBack<GroupDetailByIdResponse> callBack)
    {
       var userParticipatingGroups = await GetUserParticipatingGroups(currentUSer).ConfigureAwait(false);

       foreach (var groupCache in userParticipatingGroups)
       {
            
       }

    }
    public bool IsCacheAvailable(string groupId)
    {
        return _groupBobjCache.ContainsKey(groupId);
    }
}
