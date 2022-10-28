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
    public class UserDataService : IUserDataServices
    {
        private readonly ISqlDataServices _sqlDbAccess;

        public UserDataService(ISqlDataServices sqlDbAccess)
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
