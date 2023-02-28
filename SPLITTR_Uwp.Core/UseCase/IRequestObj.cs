using System.Threading;

namespace SPLITTR_Uwp.Core.UseCase;

public interface IRequestObj<in T>
{
    public CancellationToken Cts { get; }

    public IPresenterCallBack<T> PresenterCallBack { get; }
}
public abstract class SplittrRequestBase<T> : IRequestObj<T>
{
    protected SplittrRequestBase(CancellationToken cts, IPresenterCallBack<T> presenterCallBack)
    {
        Cts = cts;
        PresenterCallBack = presenterCallBack;
    }

    public CancellationToken Cts { get; }

    public IPresenterCallBack<T> PresenterCallBack { get; }
}
