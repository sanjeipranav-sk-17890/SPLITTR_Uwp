using System.Threading;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.Core.UseCase.GetGroupDetails;

public class GroupDetailByIdRequest : SplittrRequestBase<GroupDetailByIdResponse>
{
    public GroupDetailByIdRequest(string groupUniqueId, CancellationToken cts, IPresenterCallBack<GroupDetailByIdResponse> presenterCallBack, User currentUser) : base(cts, presenterCallBack)
    {
        GroupUniqueId = groupUniqueId;
        CurrentUser = currentUser;
    }

    public string GroupUniqueId { get; }

    public User CurrentUser { get; }

}
