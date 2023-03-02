using System.Threading.Tasks;
using SPLITTR_Uwp.Core.Adapters.NetAdapter;
using SPLITTR_Uwp.Core.DataManager;

namespace SPLITTR_Uwp.Core.NetHandler;

public class ExpenseCategoryNetHandler : IExpenseCategoryNetHandler
{
    private readonly INetAdapter _netAdapter;
    private readonly string _baseUri = "https://secure.splitwise.com/api/v3.0/";

    public ExpenseCategoryNetHandler(INetAdapter netAdapter)
    {
        _netAdapter = netAdapter;

    }
    public async Task<string> FetchExpenseCategory()
    {
        var uri = "get_categories";

        var categoryResponse = await _netAdapter.GetAsync(_baseUri, uri).ConfigureAwait(false);

        //Exception Thrown if Status Failed
        categoryResponse.EnsureSuccessStatusCode();

        return await categoryResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

    }
}
