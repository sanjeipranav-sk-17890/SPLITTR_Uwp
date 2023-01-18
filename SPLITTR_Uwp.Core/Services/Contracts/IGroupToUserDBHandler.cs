using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.Services.Contracts
{
    public interface IGroupToUserDBHandler
    {
        /// <summary>
        /// For given UserEmail(UNiqueID) fetches GroupIds Where User  is a member of the Group
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> SelectGroupIdsWithUserEmail(string userEmail);

        /// <summary>
        /// For given GroupUniqueId Fetches Participants Id from the dataBase
        /// </summary>
        /// <param name="groupUniqueId"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> SelectUserMailIdsWithGroupUniuqeId(string groupUniqueId);

        Task<int> InsertGroupMembersAsync(IEnumerable<GroupToUser> members);
    }
}
