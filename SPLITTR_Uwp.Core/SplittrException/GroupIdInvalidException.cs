using System;

namespace SPLITTR_Uwp.Core.EventArg;

public class GroupIdInvalidException : SplittrException.SplittrException
{

    public GroupIdInvalidException(Exception ex, string message) : base(ex, message)
    {
    }
    public GroupIdInvalidException(Exception ex) : base(ex)
    {
    }
}
