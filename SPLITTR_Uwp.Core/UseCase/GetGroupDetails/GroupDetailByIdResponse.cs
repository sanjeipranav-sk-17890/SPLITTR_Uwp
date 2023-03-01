using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.GetGroupDetails;

public class GroupDetailByIdResponse
{
    public GroupDetailByIdResponse(GroupBobj requestedGroup)
    {
        RequestedGroup = requestedGroup;
    }

    public GroupBobj RequestedGroup { get; }
}
