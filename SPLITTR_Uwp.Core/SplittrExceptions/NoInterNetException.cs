using System;

namespace SPLITTR_Uwp.Core.SplittrExceptions;

public class NoInterNetException : SplittrException
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
