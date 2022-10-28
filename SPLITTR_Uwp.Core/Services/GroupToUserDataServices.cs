using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Services.Contracts;
using SPLITTR_Uwp.Core.Services.SqliteConnection;

namespace SPLITTR_Uwp.Core.Services
{
    public class GroupToUserDataServices : IGroupToUserDataServices
    {
        private readonly ISqlDataServices _sqlDbAccess;

        public GroupToUserDataServices(ISqlDataServices sqlDbAccess)
        {
            _sqlDbAccess = sqlDbAccess;
            _sqlDbAccess.CreateTable<GroupToUser>();
        }

        public async Task<IEnumerable<string>> SelectGroupIdsWithUserEmail(string userEmail)
        {
            var result = await _sqlDbAccess.FetchTable<GroupToUser>().Where(g => g.UserEmailId == userEmail).ToListAsync();
            return result.Select(g => g.GroupUniqueId);
        }

        public async Task<IEnumerable<string>> SelectUserMailIdsWithGroupUniuqeId(string groupUniqueId)
        {
            var result = await _sqlDbAccess.FetchTable<GroupToUser>().Where(g => g.GroupUniqueId == groupUniqueId)
                .ToListAsync();

            return result.Select(g => g.UserEmailId);
        }

        public Task<int> InsertGroupMembersAsync(IEnumerable<GroupToUser> members)
        {
            return _sqlDbAccess.InsertObjects(members);
        }
    }
}
