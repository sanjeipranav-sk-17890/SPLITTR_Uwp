using SPLITTR_Uwp.Core.Splittr_Uwp_BLogics.Blogic.contracts;

namespace SPLITTR_Uwp.Core.DataManager.Contracts
{
    public interface IExpenseHistoryManager : IUseCase, IExpenseHistoryUsecase
    {
        /// <summary>
        /// stores History of expenses whether it is marked as Paid in Respective DataService
        /// </summary>
        /// <param name="expense"></param>
        /// <returns></returns>
        void RecordExpenseMarkedAsPaid(string expenseId);

    }
}
