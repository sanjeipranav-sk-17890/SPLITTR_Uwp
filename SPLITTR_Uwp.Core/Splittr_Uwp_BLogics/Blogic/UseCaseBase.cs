using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic;

public abstract class UseCaseBase : IUseCase
{
    public event Action<Exception, string> OnError;

    protected string _onErrorMessage;

    public CancellationTokenSource Source { get; }

    private readonly CancellationToken _token;

    public UseCaseBase()
    {
        Source = new CancellationTokenSource();
        _token = Source.Token;
    }

    protected Task RunAsynchronously(Action action)
    {
        return Task.Run(() =>
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                OnError?.Invoke(e, _onErrorMessage ?? string.Empty);
            }

        }, _token);
    }

}
