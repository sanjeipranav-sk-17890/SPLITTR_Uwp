using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Vobj;
using SPLITTR_Uwp.ViewModel.Vobj.ExpenseListObject;
using System.Collections.Generic;
using System.Linq;
using static SPLITTR_Uwp.Services.UiService;

namespace SPLITTR_Uwp.ViewModel.VmLogic;

public class ExpenseCategoryGrouper : IExpenseGrouper
{
    public ExpenseCategoryGrouper()
    {
        SplittrNotification.ExpenseCategoryChanged += SplittrNotification_ExpenseCategoryChanged;
    }


    public ICollection<ExpenseGroupingList> _previousGroupedExpenses;

    public IEnumerable<ExpenseGroupingList> CreateExpenseGroupList(IEnumerable<ExpenseVobj> expenses)
    {
        var categoryGroupedExpenses = expenses.
            GroupBy(ex => new
            {
                ex.CategoryId,
                ex.CategoryName
            }).Select(groupedCategories => new ExpenseGroupingList(groupedCategories.Key.CategoryName, groupedCategories)).ToObservableList();

        _previousGroupedExpenses = categoryGroupedExpenses;

        AssignTitlesForGroups();

        return categoryGroupedExpenses;
    }
    private void AssignTitlesForGroups()
    {
        if (!Store.Categories.Any())
        {
            Store.CategoriesLoaded += StoreOnCategoriesLoaded;
            return;
        }
        AssignHeaderTitleForGroupedList(Store.CategoryOnlyDictionary);
    }
    private void StoreOnCategoriesLoaded(CategoryLoadedEventArgs obj)
    {
        AssignHeaderTitleForGroupedList(obj.ExpenseCategoriesDict);
    }
    private void AssignHeaderTitleForGroupedList(IReadOnlyDictionary<int, ExpenseCategory> categoryOnlyDictionary)
    {
        foreach (var groupExpenses in _previousGroupedExpenses)
        {
            if (!groupExpenses.Any())
            {
                continue;
            }
            var headerCategory = categoryOnlyDictionary[groupExpenses[0].CategoryId];
            groupExpenses.GroupHeader.GroupName = headerCategory?.Name;
        }
    }

    ~ExpenseCategoryGrouper()
    {
        SplittrNotification.ExpenseCategoryChanged -= SplittrNotification_ExpenseCategoryChanged;
    }
    private void SplittrNotification_ExpenseCategoryChanged(ExpenseCategoryChangedEventArgs obj)
    {
        if (_previousGroupedExpenses is null || !_previousGroupedExpenses.Any() || obj?.ChangedCategory is null)
        {
            return;
        }
        var changedCategory = obj.ChangedCategory;
        var changedExpensObj = obj.UpdatedExpenseBobj;

        _ = RunOnUiThread(() =>
        {
            var relocatingExpense = RelocatingExpense(changedExpensObj);

            if (relocatingExpense is null)
            {
                return;
            }
            //Checking If There is a Existing Group for that Category
            var matchedGroupedExpense = _previousGroupedExpenses.FirstOrDefault(CheckIfTheGroupCategoryTypeMatches);

           
            if (matchedGroupedExpense is null)
            {
                _previousGroupedExpenses.Add(new ExpenseGroupingList(changedCategory.Name, new[] { relocatingExpense }));
            }
            else
            {
                matchedGroupedExpense.Add(relocatingExpense);
            }

        }).ConfigureAwait(false);


        bool CheckIfTheGroupCategoryTypeMatches(ExpenseGroupingList grp) => grp?.Any() is true && grp[0].CategoryId == changedCategory.Id;
    }

    private ExpenseVobj RelocatingExpense(ExpenseBobj changedExpenseObj)
    {

        ExpenseGroupingList emptyDisposeGroupObj = null;
        ExpenseVobj relocatingExpense = null;

        //Finding Changed ExpenseOBj and Returns Group if Empty
        foreach (var expenseGroup in _previousGroupedExpenses)
        {
            if (expenseGroup.Count <= 0)
            {
                continue;
            }
            //Checking Whether the Expense Belong to That Particular Group
            relocatingExpense = expenseGroup.FirstOrDefault(ex => ex.ExpenseUniqueId.Equals(changedExpenseObj.ExpenseUniqueId));

            if (relocatingExpense is null)
            {
                continue;
            }
            //Remove From The Existing Group for Relocation Purpose
            expenseGroup.Remove(relocatingExpense);

            //if no Object Exist grouped List It will Be Removed From one of the many Grouping List
            emptyDisposeGroupObj = expenseGroup.Any() ? null : expenseGroup;
            break;
        }
        if (emptyDisposeGroupObj is not null)
        {
            _previousGroupedExpenses.Remove(emptyDisposeGroupObj);
        }
        return relocatingExpense;
    }

}
