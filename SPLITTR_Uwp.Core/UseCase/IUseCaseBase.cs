namespace SPLITTR_Uwp.Core.UseCase;

public interface IUseCaseBase
{
    public void Execute();
    public bool GetIfAvailableFromCache();

}
