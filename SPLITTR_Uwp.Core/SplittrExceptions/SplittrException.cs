using System;
using System.Security.Cryptography.X509Certificates;

namespace SPLITTR_Uwp.Core.SplittrExceptions;

public class SplittrException : Exception
{
    public bool IsNetworkCallError { get; internal set; }

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