using System.Collections.Concurrent;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetGroupDetails;

namespace SPLITTR_Uwp.Core.DataManager;

public class GroupDetailManager : GroupDataManagerBase, IGroupDetailManager
{

    private readonly ConcurrentDictionary<string, GroupBobj> _groupBobjCache = new ConcurrentDictionary<string, GroupBobj>();

    public GroupDetailManager(IGroupDbHandler groupDbHandler, IUserDataManager userDataManager, IGroupToUserDbHandler groupToUserDbHandler) : base(groupDbHandler, groupToUserDbHandler, userDataManager)
    {

    }

    public async void FetchGroupByGroupId(string groupId, User currentUSer, IUseCaseCallBack<GroupDetailByIdResponse> callBack)
    {
        if (ReturnIfCacheAvailable(groupId))
        {
            return;
        }

        var userParticipatingGroups = await GetUserParticipatingGroups(currentUSer).ConfigureAwait(false);

        foreach (var groupBobj in userParticipatingGroups)
        {
            _groupBobjCache.AddOrUpdate(groupBobj.GroupUniqueId, groupBobj, (s, bobj) => bobj);
        }
        if (!ReturnIfCacheAvailable(groupId))
        {
            callBack?.OnError(new GroupIdInvalidException(null, $"Group with Id {groupId} not found"));
        }
        bool ReturnIfCacheAvailable(string groupId)
        {
            if (!_groupBobjCache.ContainsKey(groupId))
            {
                return false;
            }
            callBack?.OnSuccess(new GroupDetailByIdResponse(_groupBobjCache[groupId]));
            return true;
        }
    }
    public bool IsCacheAvailable(string groupId)
    {
        return _groupBobjCache.ContainsKey(groupId);
    }
}