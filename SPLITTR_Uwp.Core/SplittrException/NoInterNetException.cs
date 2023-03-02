using System;

namespace SPLITTR_Uwp.Core.EventArg;

public class NoInterNetException : SplittrException.SplittrException
{
    public NoInterNetException()
    {
        
    }
    public NoInterNetException(Exception ex, string message) : base(ex, message)
    {
    }
    public NoInterNetException(Exception ex) : base(ex)
    {
    }

}
