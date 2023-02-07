using System.Collections.Generic;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DbHandler.Contracts
{
    public interface IUserDbHandler
    {
        Task<User> SelectUserObjByEmailId(string emailId);
        Task<int> InsertUserObjAsync(User user);
        Task UpDateUserAsync(User user);
        Task<IEnumerable<User>> SelectUserFormUsers(string userName);
    }
}
