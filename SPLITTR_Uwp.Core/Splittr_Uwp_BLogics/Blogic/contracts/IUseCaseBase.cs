using System;
using System.Threading;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic.contracts
{
    public interface IUseCase
    {
        event Action<Exception, string> OnError;

        CancellationTokenSource Source { get; }

        
    }
}
