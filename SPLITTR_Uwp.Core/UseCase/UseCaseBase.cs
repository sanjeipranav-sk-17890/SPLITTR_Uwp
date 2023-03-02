using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPLITTR_Uwp.Core.UseCase;

public abstract class UseCaseBase<T> : IUseCaseBase
{
    private  CancellationToken _cts;

    public IPresenterCallBack<T> PresenterCallBack { get; set; }

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
                var exception = new SplittrException.SplittrException(e, e.Message);
                PresenterCallBack?.OnError(exception);
            }
        },_cts);
    }
    protected abstract void Action();
      
    public virtual bool GetIfAvailableFromCache()
    {
        return false;
    }
}
