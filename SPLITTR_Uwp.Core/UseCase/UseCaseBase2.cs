using System.Collections.Generic;
using System.Text;
using SPLITTR_Uwp.Core.EventArg;

namespace SPLITTR_Uwp.Core.UseCase
{
    public interface IUseCaseBase
    {
        public void Execute();
        public void Action();
        public bool GetIfAvailableFromCache();

    }
    public interface IPresenterCallBack<in T>
    {
        void OnSuccess(T result);
        void OnError(SplittrException ex);
    }
    public interface IPresenterCallBack
    {
        void OnSuccess();
        void OnError(SplittrException ex);
    }
}
