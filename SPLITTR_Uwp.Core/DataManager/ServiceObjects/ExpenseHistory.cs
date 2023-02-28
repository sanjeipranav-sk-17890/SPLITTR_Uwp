using System;
using SQLite;

namespace SPLITTR_Uwp.Core.DataManager.ServiceObjects;

public class ExpenseHistory 
{
    [PrimaryKey,AutoIncrement]
    public int Id { get; set; }

    public string ExpenseUniqueId { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsMarkedAsPaid { get; set; }=false;

    public ExpenseHistory(string expenseUniqueId):this()
    {
        ExpenseUniqueId = expenseUniqueId;
    }
    public ExpenseHistory()
    {
        CreatedDate = DateTime.Now;
    }

}