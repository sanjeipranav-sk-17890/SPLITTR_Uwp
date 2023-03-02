using SPLITTR_Uwp.Core.SplittrExceptions;

namespace SPLITTR_Uwp.Core.UseCase;

public interface IUseCaseCallBack<in T>
{
    public void OnSuccess(T responseObj);
    public void OnError(SplittrException error);

}