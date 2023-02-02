using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.CreateGroup;
using SQLite;

namespace SPLITTR_Uwp.Core.DataManager
{
    public class GroupCreationDataManager : IGroupCreationDataManager
    {
        private readonly IGroupDataManager _groupDataManager;
        public GroupCreationDataManager(IGroupDataManager groupDataManager)
        {
            _groupDataManager = groupDataManager;

        }
        /// <exception cref="ArgumentNullException"><paramref name="collection">collection</paramref> is null.</exception>
        /// <exception cref="ArgumentException">Group Participants Must be Grater than 2</exception>
        public async void CreateSplittrGroup(IEnumerable<User> particiapants, UserBobj currentUser, string groupName, IUseCaseCallBack<GroupCreationResponseObj> callBack)
        {
            try
            {
                if (!particiapants.Any())
                {
                    throw new ArgumentException($"number of {nameof(particiapants)} atleast  must be 2 To form a group");
                }

                var groupParticipants = new List<User>()
                {
                    currentUser
                };
                groupParticipants.AddRange(particiapants); //adding current user as one of the participants

                var newGroup = new GroupBobj(new Group
                {
                    UserEmailId = currentUser.EmailId,
                    GroupName = groupName.Trim()
                }, groupParticipants);


                //saving group Data to Data Repos
                await _groupDataManager.CreateGroupAsync(newGroup).ConfigureAwait(false);

                //adding New Group obj to group UserBobj which will raise valueChanged event
                currentUser.Groups.Add(newGroup);

                //Passing Response to UseCase CallBack
                var responseObj = CreateResponseObject(newGroup);
                callBack?.OnSuccess(responseObj);
            }
            catch (ArgumentException ex)
            {
                var error = new SplittrException(ex, ex.Message);
                callBack.OnError(error);
            }
            catch (SQLiteException ex)
            {
                var error = new SplittrException(ex, ex.Message);
                callBack.OnError(error);
            }
            catch (Exception ex)
            {
                var error = new SplittrException(ex, ex.Message);
                callBack?.OnError(error);
            }

        }


        private GroupCreationResponseObj CreateResponseObject(GroupBobj groupBobj)
        {
            return new GroupCreationResponseObj(groupBobj);
        }
    }
}
