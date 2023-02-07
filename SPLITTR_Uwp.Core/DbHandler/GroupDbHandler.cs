using System;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.DbHandler.SqliteConnection;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DbHandler
{
    public class GroupDbHandler :IGroupDbHandler
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
