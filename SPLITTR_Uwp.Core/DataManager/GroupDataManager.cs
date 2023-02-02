using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DataManager
{
    public class GroupDataManager : IGroupDataManager
    {
        private readonly IGroupDbHandler _groupDbHandler;
        private readonly IGroupToUserDbHandler _groupToUserDbHandler;
        private  IUserDataManager _userDataManager;

        public GroupDataManager(IGroupDbHandler groupDbHandler, IGroupToUserDbHandler groupToUserDbHandler)
        {
            _groupDbHandler = groupDbHandler;
            _groupToUserDbHandler = groupToUserDbHandler;
            
        }
        public async Task<ICollection<GroupBobj>> GetUserPartcipatingGroups(User user, IUserDataManager userDataManager)
        {
            _userDataManager = userDataManager; 
            if (string.IsNullOrEmpty(user.EmailId))
            {
                throw new ArgumentNullException(nameof(user.EmailId),"UserBobj's value must be initialized first");
            }

            //fetching groupIds where user is a Participant 
            var groupIds = await _groupToUserDbHandler.SelectGroupIdsWithUserEmail(user.EmailId).ConfigureAwait(false);


            var outputList = new List<GroupBobj>();


            //For each groupIds Asyncronously Fetcing the GroupBojs's of the respective groupIds

            

            foreach (var grpIds in groupIds)
            {
                var group = await _groupDbHandler.GetGroupObjByGroupId(grpIds).ConfigureAwait(false);


                var participants = await GetGroupParticipants(group.GroupUniqueId).ConfigureAwait(false);

                outputList.Add(new GroupBobj(group, participants));

            }

           
            return outputList;

        }


        //var result = Parallel.ForEach(groupIds, (async s =>
        //{
        //    var group = await _groupDbHandler.GetGroupObjByGroupId(s);

        //    var participants = await GetGroupParticipants(group.GroupUniqueId);

        //    outputList.Add(new GroupBobj(this, group, participants));
        //})).IsCompleted;


        public Task CreateGroupAsync(GroupBobj groupBobj)
        {
            _groupDbHandler.InsertGroupAsync(groupBobj);

            var groupParticipants = groupBobj.GroupParticipants.Select(u => new GroupToUser(u.EmailId,  groupBobj.GroupUniqueId));

            return _groupToUserDbHandler.InsertGroupMembersAsync(groupParticipants);
        }


        /// <summary>
        /// for given GroupUniqueId Retrives the all the participants of the group
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




        //Parallel.ForEach(groupParticipantsIds, (async (string s) =>
        //{
        //    var userObj = await _userDataManager.SelectUserObjByEmailId(s).ConfigureAwait(false);
        //    participants.Add(userObj);
        //}));

    }
}
