using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    public abstract class UseCaseBase<T> : IUseCaseBase
    {
        private  CancellationToken _cts;

        protected IPresenterCallBack<T> PresenterCallBack { get; set; }

        protected UseCaseBase(IPresenterCallBack<T> callBack,CancellationToken token)
        {
            _cts = token;
            PresenterCallBack = callBack;

        }

        public void Execute()
        {
            if (GetIfAvailableFromCache())
            {
                return;
            }
            Task.Run(() =>
            {
                try
                {
                    Action();
                }
                catch (Exception e)
                {
                    var exception = new SplittrException(e, e.Message);
                    PresenterCallBack?.OnError(exception);
                }
            },_cts);
        }
        public abstract void Action();
      
        public virtual bool GetIfAvailableFromCache()
        {
            return false;
        }
    }
}
