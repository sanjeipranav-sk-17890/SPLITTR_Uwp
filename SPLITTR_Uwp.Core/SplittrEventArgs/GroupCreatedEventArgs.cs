using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.SplittrEventArgs;

public class GroupCreatedEventArgs : SplittrEventArgs
{
    public GroupCreatedEventArgs(GroupBobj createdGroup)
    {
        CreatedGroup = createdGroup;
    }

    public GroupBobj CreatedGroup { get; }
}
