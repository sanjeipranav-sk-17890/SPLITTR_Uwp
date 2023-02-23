using System.Collections.Generic;
using System.Text;
using SPLITTR_Uwp.Core.EventArg;

namespace SPLITTR_Uwp.Core.UseCase
{
    public interface IPresenterCallBack<in T>
    {
        void OnSuccess(T result);
        void OnError(SplittrException ex);
    }
}
