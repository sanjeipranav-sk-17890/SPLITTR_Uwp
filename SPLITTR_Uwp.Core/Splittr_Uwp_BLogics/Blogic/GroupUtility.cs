using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic
{
    public class GroupUtility :UseCaseBase, IGroupUtility
    {
        private readonly IGroupDataHandler _groupDataHandler;
        public GroupUtility(IGroupDataHandler groupDataHandler)
        {
            _groupDataHandler = groupDataHandler;

        }
        public void CreateSplittrGroup(IEnumerable<User> particiapants, UserBobj currentUser, string groupName, Action onSuccessCallBack)
        { 

            
            RunAsynchronously(() =>
            {
                if (!particiapants.Any())
                {
                    throw new ArgumentException($"number of {nameof(particiapants)} atleast  must be 1 To form a group");
                }

                var groupParticipants = new List<User>()
                {
                    currentUser
                };
                groupParticipants.AddRange(particiapants);//adding current user as one of the participants

                var newGroup = new GroupBobj(new Group
                {
                    UserEmailId = currentUser.EmailId, GroupName = groupName.Trim()
                }, groupParticipants);


                //saving group Data to Data Repos
                 _groupDataHandler.CreateGroupAsync(newGroup);

                //adding New Group obj to group UserBobj which will raise valuechanged event
                currentUser.Groups.Add(newGroup);

                
                //invoking call Back functions
                onSuccessCallBack?.Invoke();

            });
        }
    }
}
