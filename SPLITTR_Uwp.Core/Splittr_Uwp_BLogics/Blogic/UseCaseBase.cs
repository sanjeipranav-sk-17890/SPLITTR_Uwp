using System;
using System.Threading;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic.contracts;

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

    protected void RunAsynchronously(Action action)
    {
        Task.Run(() =>
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
