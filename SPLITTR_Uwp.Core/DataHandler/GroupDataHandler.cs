using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Services.Contracts;

namespace SPLITTR_Uwp.Core.DataHandler
{
    public class GroupDataHandler : IGroupDataHandler
    {
        private readonly IGroupDataServices _groupDataServices;
        private readonly IGroupToUserDataServices _groupToUserDataServices;
        private readonly IUserDataServices _userDataServices;

        public GroupDataHandler(IGroupDataServices groupDataServices, IGroupToUserDataServices groupToUserDataServices, IUserDataServices userDataServices)
        {
            _groupDataServices = groupDataServices;
            _groupToUserDataServices = groupToUserDataServices;
            _userDataServices = userDataServices;
        }
        public async Task<ICollection<GroupBobj>> GetUserPartcipatingGroups(User currentUser)
        {
            if (string.IsNullOrEmpty(currentUser.EmailId))
            {
                throw new ArgumentNullException(nameof(currentUser.EmailId),"UserBobj's value must be initialized first");
            }

            //fetching groupIds where user is a Participant 
            var groupIds = await _groupToUserDataServices.SelectGroupIdsWithUserEmail(currentUser.EmailId).ConfigureAwait(false);


            var outputList = new List<GroupBobj>();


            //For each groupIds Asyncronously Fetcing the GroupBojs's of the respective groupIds

            

            foreach (var grpIds in groupIds)
            {
                var group = await _groupDataServices.GetGroupObjByGroupId(grpIds).ConfigureAwait(false);


                var participants = await GetGroupParticipants(group.GroupUniqueId).ConfigureAwait(false);

                outputList.Add(new GroupBobj(this, group, participants));

            }

           
            return outputList;

        }


        //var result = Parallel.ForEach(groupIds, (async s =>
        //{
        //    var group = await _groupDataServices.GetGroupObjByGroupId(s);

        //    var participants = await GetGroupParticipants(group.GroupUniqueId);

        //    outputList.Add(new GroupBobj(this, group, participants));
        //})).IsCompleted;


        public Task CreateGroupAsync(GroupBobj groupBobj)
        {
            _groupDataServices.InsertGroupAsync(groupBobj);

            var groupParticipants = groupBobj.GroupParticipants.Select(u => new GroupToUser(u.EmailId,  groupBobj.GroupUniqueId));

            return _groupToUserDataServices.InsertGroupMembersAsync(groupParticipants);
        }


        /// <summary>
        /// for given GroupUniqueId Retrives the all the participants of the group
        /// </summary>
        /// <param name="groupUniqueId"></param>
        /// <returns></returns>
        private async Task<IEnumerable<User>> GetGroupParticipants(string groupUniqueId)
        {
            var groupParticipantsIds = await _groupToUserDataServices.SelectUserMailIdsWithGroupUniuqeId(groupUniqueId).ConfigureAwait(false);
           
            List<Task<User>> userObjs = new List<Task<User>>();
            foreach (var participantsId in groupParticipantsIds)
            {
                userObjs.Add(_userDataServices.SelectUserObjByEmailId(participantsId));
            }

            return  await Task.WhenAll(userObjs).ConfigureAwait(false);
            
        }




        //Parallel.ForEach(groupParticipantsIds, (async (string s) =>
        //{
        //    var userObj = await _userDataServices.SelectUserObjByEmailId(s).ConfigureAwait(false);
        //    participants.Add(userObj);
        //}));

    }
}
