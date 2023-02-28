using SPLITTR_Uwp.Core.ModelBobj;

namespace SPLITTR_Uwp.Core.UseCase.MarkAsPaid;

public class MarkAsPaidResponseObj
{
    public  ExpenseBobj MarkedPaidExpenseBobj { get;}

    public MarkAsPaidResponseObj(ExpenseBobj markedExpenseBobj)
    {
        MarkedPaidExpenseBobj = markedExpenseBobj;
    }

}