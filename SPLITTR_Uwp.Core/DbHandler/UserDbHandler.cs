using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.DbHandler.Contracts;
using SPLITTR_Uwp.Core.DbHandler.SqliteConnection;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DbHandler
{
    public class UserDbHandler : IUserDbHandler
    {
        private readonly ISqlDataServices _sqlDbAccess;

        public UserDbHandler(ISqlDataServices sqlDbAccess)
        {
            _sqlDbAccess = sqlDbAccess; 
           _sqlDbAccess.CreateTable<User>();
        }

        public Task<User> SelectUserObjByEmailId(string emailId)
        {
           return _sqlDbAccess.FetchTable<User>().Where(u => u.EmailId == emailId).FirstOrDefaultAsync();
        }

        public Task<int> InsertUserObjAsync(User user)
        {
            return _sqlDbAccess.InsertObj(user);
        }
        public Task UpDateUserAsync(User user)
        {
            return _sqlDbAccess.UpdateObj(user);
        }
        public async Task<IEnumerable<User>> SelectUserFormUsers(string userName)
        {
            var result = await _sqlDbAccess.FetchTable<User>().ToListAsync().ConfigureAwait(false);
            return result.Where(FilterUser);

            bool FilterUser(User user)
            {
                var name = user.UserName.ToLower();
                return name.StartsWith(userName.Trim());
            }
        }
      
    }
}
