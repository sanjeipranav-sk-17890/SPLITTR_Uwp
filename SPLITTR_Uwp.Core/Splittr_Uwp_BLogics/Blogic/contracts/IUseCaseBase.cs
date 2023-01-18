using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic
{
    public interface IUseCase
    {
        event Action<Exception, string> OnError;

        CancellationTokenSource Source { get; }

        
    }
}
