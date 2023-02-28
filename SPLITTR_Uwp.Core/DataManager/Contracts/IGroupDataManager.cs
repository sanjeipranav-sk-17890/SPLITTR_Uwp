using System.Threading.Tasks;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetUserGroups;

namespace SPLITTR_Uwp.Core.DataManager.Contracts
{
    public interface IGroupDataManager
    {
        /// <summary>
        /// perform join operation on  Group table ,groupToUser table,User table based upon Provided User Email Id
        /// And returns Collection of Group obj
        /// </summary>
        /// <param name="user">Current User entity object</param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        void GetUserParticipatingGroups(User user,IUseCaseCallBack<GetUserGroupResponse> callBack);

        Task CreateGroupAsync(GroupBobj groupBobj);
    }
}
