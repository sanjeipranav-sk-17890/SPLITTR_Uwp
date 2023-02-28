using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Adapters.SqlAdapter;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DbHandler;

public class GroupToUserDbHandler : IGroupToUserDbHandler
{
    private readonly ISqlDataAdapter _sqlDbAccess;

    public GroupToUserDbHandler(ISqlDataAdapter sqlDbAccess)
    {
        _sqlDbAccess = sqlDbAccess;
        _sqlDbAccess.CreateTable<GroupToUser>();
    }

    public async Task<IEnumerable<string>> SelectGroupIdsWithUserEmail(string userEmail)
    {
        var result = await _sqlDbAccess.FetchTable<GroupToUser>().Where(g => g.UserEmailId == userEmail).ToListAsync().ConfigureAwait(false);
        return result.Select(g => g.GroupUniqueId);
    }

    public async Task<IEnumerable<string>> SelectUserMailIdsWithGroupUniuqeId(string groupUniqueId)
    {
        var result = await _sqlDbAccess.FetchTable<GroupToUser>().Where(g => g.GroupUniqueId == groupUniqueId)
            .ToListAsync().ConfigureAwait(false);

        return result.Select(g => g.UserEmailId);
    }

    public Task<int> InsertGroupMembersAsync(IEnumerable<GroupToUser> members)
    {
        return _sqlDbAccess.InsertObjects(members);
    }
}