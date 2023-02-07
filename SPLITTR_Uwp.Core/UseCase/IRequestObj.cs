using System.Threading;

namespace SPLITTR_Uwp.Core.UseCase;

public interface IRequestObj<in T>
{
    public CancellationToken Cts { get; }

    public IPresenterCallBack<T> PresenterCallBack { get; }
}
