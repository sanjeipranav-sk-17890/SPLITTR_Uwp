#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.ModelBobj
{
    public class GroupBobj : Group
    {

        
        private readonly IList<User> _groupParticipants = new List<User>();

        public virtual IList<User> GroupParticipants
        {
            get => _groupParticipants;
        }

        public User? CreatedOwner
        {
            get => _groupParticipants.FirstOrDefault(u => u.EmailId == UserEmailId);
        }


        private GroupBobj(Group group) : base(group.UserEmailId,group.GroupUniqueId,group.GroupName,group.CreateDateTime)
        {

        }

        public GroupBobj(Group group ,IEnumerable<User> groupParticipants):this(group)
        {
            
            _groupParticipants.AddRange(groupParticipants);
        }

        
        protected GroupBobj(GroupBobj groupBobj) : this(groupBobj,groupBobj.GroupParticipants)
        {

        }

        public GroupBobj() // For creating Dummy Data
        {

        }
    }
}
