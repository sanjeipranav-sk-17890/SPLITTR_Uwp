using System;

namespace SPLITTR_Uwp.Core.SplittrExceptions;

public class SplittrException : Exception
{
    public SplittrException(Exception ex, string message):base(message,ex)
    {

    }
    public SplittrException(Exception ex) : this(ex,ex.Message)
    {

    }
    protected SplittrException()
    {
        
    }
}