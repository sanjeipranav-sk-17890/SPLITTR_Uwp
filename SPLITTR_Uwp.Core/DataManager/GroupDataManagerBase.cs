using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetUserGroups;

namespace SPLITTR_Uwp.Core.DataManager;

public class GroupDataManagerBase : IGroupDataManager
{
    private readonly IGroupDbHandler _groupDbHandler;
    private readonly IGroupToUserDbHandler _groupToUserDbHandler;
    private readonly IUserDataManager _userDataManager;

    public GroupDataManagerBase(IGroupDbHandler groupDbHandler, IGroupToUserDbHandler groupToUserDbHandler,IUserDataManager userDataManager)
    {
        _groupDbHandler = groupDbHandler;
        _groupToUserDbHandler = groupToUserDbHandler;
        _userDataManager = userDataManager;
            
    }
    
    async void IGroupDataManager.GetUserParticipatingGroups(User user,IUseCaseCallBack<GetUserGroupResponse> callBack)
    {

        try
        {
            var outputList = await GetUserParticipatingGroups(user).ConfigureAwait(false);

            callBack?.OnSuccess(new GetUserGroupResponse(outputList));

        }
        catch(Exception e)
        {
            callBack?.OnError(new SplittrException(e,"Db Call Failed"));
        }
    }


    protected async Task<List<GroupBobj>> GetUserParticipatingGroups(User user)
    {

        if (string.IsNullOrEmpty(user.EmailId))
        {
            throw new ArgumentNullException(nameof(user.EmailId), "UserBobj  value must be initialized first");
        }

        //fetching groupIds where user is a Participant 
        var groupIds = await _groupToUserDbHandler.SelectGroupIdsWithUserEmail(user.EmailId).ConfigureAwait(false);


        var outputList = new List<GroupBobj>();


        //For each groupIds Asynchronously Fetching the GroupBoj of the respective groupIds

        foreach (var grpIds in groupIds)
        {
            var group = await _groupDbHandler.GetGroupObjByGroupId(grpIds).ConfigureAwait(false);


            var participants = await GetGroupParticipants(group.GroupUniqueId).ConfigureAwait(false);

            outputList.Add(new GroupBobj(group, participants));

        }
        return outputList;
    }


    Task IGroupDataManager.CreateGroupAsync(GroupBobj groupBobj)
    {
        _groupDbHandler.InsertGroupAsync(groupBobj);

        var groupParticipants = groupBobj.GroupParticipants.Select(u => new GroupToUser(u.EmailId,  groupBobj.GroupUniqueId));

        return _groupToUserDbHandler.InsertGroupMembersAsync(groupParticipants);
    }

    /// <summary>
    /// for given GroupUniqueId Retrieves the all the participants of the group
    /// </summary>
    /// <param name="groupUniqueId"></param>
    /// <returns></returns>
    private async Task<IEnumerable<User>> GetGroupParticipants(string groupUniqueId)
    {
        var groupParticipantsIds = await _groupToUserDbHandler.SelectUserMailIdsWithGroupUniuqeId(groupUniqueId).ConfigureAwait(false);
           
        List<Task<User>> userObjs = new List<Task<User>>();
        foreach (var participantsId in groupParticipantsIds)
        {
            userObjs.Add(_userDataManager.FetchUserUsingMailId(participantsId));
        }

        return  await Task.WhenAll(userObjs).ConfigureAwait(false);
            
    }

}