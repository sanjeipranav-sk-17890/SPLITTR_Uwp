using System;
using System.Collections.Generic;
using System.Text;

namespace SPLITTR_Uwp.Core.EventArg
{
    public class SplittrException : Exception
    {
        public SplittrException(Exception ex, string message):base(message,ex)
        {
        }
    }
    public interface IPresenterCallBack<in T>
    {
        void OnSuccess(T result);
        void OnError(SplittrException ex);
    }
}
