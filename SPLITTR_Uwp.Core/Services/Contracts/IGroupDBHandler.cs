using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.Services.Contracts
{
    public interface IGroupDBHandler
    {
        Task<Group> GetGroupObjByGroupId(string groupId);
        Task<int> InsertGroupAsync(Group group);
    }
}
