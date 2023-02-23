namespace SPLITTR_Uwp.Core.UseCase;

public interface IUseCaseBase
{
    public void Execute();
    public void Action();
    public bool GetIfAvailableFromCache();

}
