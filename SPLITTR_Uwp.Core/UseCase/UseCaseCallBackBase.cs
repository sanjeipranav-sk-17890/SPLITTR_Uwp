using SPLITTR_Uwp.Core.SplittrExceptions;

namespace SPLITTR_Uwp.Core.UseCase;

public class UseCaseCallBackBase<T> : IUseCaseCallBack<T>
{
    private readonly UseCaseBase<T> _useCase;
    public UseCaseCallBackBase(UseCaseBase<T> useCase)
    {
        _useCase = useCase;

    }
    public virtual void OnSuccess(T result)
    {
        _useCase?.PresenterCallBack.OnSuccess(result);
    }
    public virtual void OnError(SplittrException ex)
    {
        _useCase?.PresenterCallBack.OnError(ex);
    }
}
