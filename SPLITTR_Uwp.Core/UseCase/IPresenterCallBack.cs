using SPLITTR_Uwp.Core.SplittrExceptions;

namespace SPLITTR_Uwp.Core.UseCase;

public interface IPresenterCallBack<in T>
{
    void OnSuccess(T result);
    void OnError(SplittrException ex);
}