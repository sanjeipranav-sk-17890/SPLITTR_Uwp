using SPLITTR_Uwp.Core.ModelBobj;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DataHandler.Contracts
{
    public interface IGroupDataHandler
    {
        /// <summary>
        /// perform join operation on  Group table ,groupToUser table,User table based upon Provided User Email Id
        /// And returns Collection of Group obj
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        Task<ICollection<GroupBobj>> GetUserPartcipatingGroups(User currentUser);

        Task CreateGroupAsync(GroupBobj groupBobj);
    }
}
