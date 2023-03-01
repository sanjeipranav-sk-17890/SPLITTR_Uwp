using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetGroupDetails;

namespace SPLITTR_Uwp.Core.DataManager.Contracts;

public interface IGroupDetailManager
{
    public void FetchGroupByGroupId(string groupId, User currentUSer, IUseCaseCallBack<GroupDetailByIdResponse> callBack);
    public bool IsCacheAvailable(string groupId);
}