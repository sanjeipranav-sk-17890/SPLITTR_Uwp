using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.CreateGroup;

public class GroupCreationResponseObj
{
    public GroupCreationResponseObj(GroupBobj createdGroup)
    {
        CreatedGroup = createdGroup;
    }

    public GroupBobj CreatedGroup { get; set; }
}
