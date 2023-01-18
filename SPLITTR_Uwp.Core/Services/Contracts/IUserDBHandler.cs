using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.Services.Contracts
{
    public interface IUserDBHandler
    {
        Task<User> SelectUserObjByEmailId(string emailId);
        Task<int> InsertUserObjAsync(User user);
        Task UpDateUserAsync(User user);
        Task<IEnumerable<User>> SelectUserFormUsers(string userName);
    }
}
