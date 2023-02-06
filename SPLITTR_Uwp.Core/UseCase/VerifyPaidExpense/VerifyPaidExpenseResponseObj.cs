namespace SPLITTR_Uwp.Core.UseCase.VerifyPaidExpense;

public class VerifyPaidExpenseResponseObj
{
    public VerifyPaidExpenseResponseObj(bool isExpenseMarkAsPaid)
    {
        IsExpenseMarkAsPaid = isExpenseMarkAsPaid;
    }

    public bool IsExpenseMarkAsPaid { get; }
}
