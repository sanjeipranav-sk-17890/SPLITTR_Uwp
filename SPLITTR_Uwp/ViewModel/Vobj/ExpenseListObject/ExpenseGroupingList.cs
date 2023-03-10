using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using static SPLITTR_Uwp.Services.UiService;

namespace SPLITTR_Uwp.ViewModel.Vobj.ExpenseListObject;

public class ExpenseGroupingList :ObservableCollection<ExpenseVobj>
{
    
    public ExpenseGroupHeader GroupHeader { get; }
       
        
    public ExpenseGroupingList(string headerTitle, IEnumerable<ExpenseBobj> expenses)
    {
        GroupHeader = new ExpenseGroupHeader(this, headerTitle);

        foreach (var expense in expenses)
        {
            if (expense == null)
            {
                continue;
            }
            if (expense is ExpenseVobj vobj)
            {
                Add(vobj);
                continue;
            }
            Add(new ExpenseVobj(expense));
        }
        base.CollectionChanged += ExpenseGroupingList_CollectionChanged;

    }

    private void ExpenseGroupingList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        base.OnPropertyChanged(new PropertyChangedEventArgs(nameof(ExpenseGroupHeader)));  
    }

    public override bool Equals(object obj)
    {
        if (obj is not ExpenseGroupingList expenseGroup)
        {
            return false;
        }
        return expenseGroup.GroupHeader.GroupName?.Equals(GroupHeader.GroupName) is true;

    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}
public class ExpenseGroupHeader : INotifyPropertyChanged
{
    private readonly ObservableCollection<ExpenseVobj> _expense;
    private string _groupName;

    public string GroupName
    {
        get => _groupName;
        set => SetField(ref _groupName, value);
    }

    public ExpenseGroupHeader(ObservableCollection<ExpenseVobj>expense,string groupNAme)
    {
        GroupName = groupNAme??"...";
        _expense = expense;
        expense.CollectionChanged += Expense_CollectionChanged;

    }

    private void Expense_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(NoOfExpenses));
    }

    public int NoOfExpenses
    {
        get => _expense.Count;
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected async void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
       await RunOnUiThread(() =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }).ConfigureAwait(false);
    }
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}