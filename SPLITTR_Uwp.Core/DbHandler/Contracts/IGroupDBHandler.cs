using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.DbHandler.Contracts
{
    public interface IGroupDbHandler
    {
        Task<Group> GetGroupObjByGroupId(string groupId);
        Task<int> InsertGroupAsync(Group group);
    }
}
