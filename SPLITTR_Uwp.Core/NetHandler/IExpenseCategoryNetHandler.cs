using System.Threading.Tasks;

namespace SPLITTR_Uwp.Core.NetHandler;

public interface IExpenseCategoryNetHandler
{
    public Task<string> FetchExpenseCategory();
}
