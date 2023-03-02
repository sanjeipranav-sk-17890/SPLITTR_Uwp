using System.Threading.Tasks;

namespace SPLITTR_Uwp.Core.DataManager;

public interface IExpenseCategoryNetHandler
{
    public Task<string> FetchExpenseCategory();
}
