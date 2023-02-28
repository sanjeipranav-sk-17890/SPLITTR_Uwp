using System;

namespace SPLITTR_Uwp.Core.EventArg
{
    public class SplittrException : Exception
    {
        public SplittrException(Exception ex, string message):base(message,ex)
        {
        }
        public SplittrException(Exception ex)
        {

        }
    }
}
