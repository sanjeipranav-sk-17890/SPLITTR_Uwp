using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.Services.Contracts;
using SPLITTR_Uwp.Core.Services.SqliteConnection;

namespace SPLITTR_Uwp.Core.Services
{
    public class GroupDbHandler :IGroupDBHandler
    {
        private readonly ISqlDataServices _sqlDbAccess;

        public GroupDbHandler(ISqlDataServices sqlDbAccess)
        {
            _sqlDbAccess = sqlDbAccess;
            _sqlDbAccess.CreateTable<Group>();
        }

        public Task<Group> GetGroupObjByGroupId(string groupId)
        {
           return _sqlDbAccess.FetchTable<Group>().Where(g => g.GroupUniqueId.Equals(groupId)).FirstOrDefaultAsync() ??
                throw new NotSupportedException("Group Not Found by GroupId");
        }

        public Task<int> InsertGroupAsync(Group group)
        {
            return _sqlDbAccess.InsertObj(group);
        }
    }
}
