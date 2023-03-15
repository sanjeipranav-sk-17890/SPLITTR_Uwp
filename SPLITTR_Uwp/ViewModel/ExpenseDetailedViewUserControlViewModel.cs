using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.ChangeExpenseCategory;
using SPLITTR_Uwp.Core.UseCase.EditExpense;
using SPLITTR_Uwp.Core.UseCase.GetGroupDetails;
using SPLITTR_Uwp.Core.UseCase.GetRelatedExpense;
using SPLITTR_Uwp.Core.UseCase.GetUserGroups;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Vobj;
using static SPLITTR_Uwp.Services.UiService;

namespace SPLITTR_Uwp.ViewModel;

internal class ExpenseDetailedViewUserControlViewModel : ObservableObject
{

    #region Bindable_Properties
    private double _totalExpenditureAmount;

    public double TotalExpenditureAmount
    {
        get => _totalExpenditureAmount;
        set => SetProperty(ref _totalExpenditureAmount, value);
    }
    public ObservableCollection<ExpenseVobj> RelatedExpenses { get; } = new ObservableCollection<ExpenseVobj>();

    private string _expenseOccuredGroupName = string.Empty;


    private bool _hasEditAccess;
    private string _expenseOwnerInfo;
    private string _expenseTitle;
    private DateTimeOffset _dateOfExpense;
    private string _expenseNote;



    public bool HasEditAccess
    {
        get => _hasEditAccess;
        set => SetProperty(ref _hasEditAccess, value);
    }

    public string ExpenseOwnerInfo
    {
        get => _expenseOwnerInfo;
        set => SetProperty(ref _expenseOwnerInfo, value);
    }

    public string ExpenseTitle
    {
        get => _expenseTitle;
        set => SetProperty(ref _expenseTitle, value);
    }

    public DateTimeOffset DateOfExpense
    {
        get => _dateOfExpense;
        set => SetProperty(ref _dateOfExpense, value);
    }

    public string ExpenseNote
    {
        get => _expenseNote;
        set => SetProperty(ref _expenseNote, value);
    }

    public string ExpenseOccuredGroupName
    {
        get => _expenseOccuredGroupName;
        set => SetProperty(ref _expenseOccuredGroupName, value);
    }

    #endregion

    //Storing Reference Of passed Expense ,utilized to make Ui manupulation
    private ExpenseVobj _expense;


    private void PopulatePropertyData(ExpenseVobj expense)
    {
        HasEditAccess = IsCurrentUserRaisedExpense(expense);
        ExpenseOwnerInfo = FormatExpenseOwnerInfo(expense);

        //No UseCase Should Be Called While Assigning Initial value to Property
        _isAssignTimePropertyChange = true;

        ExpenseNote = expense?.Note ?? string.Empty;
        DateOfExpense = expense?.DateOfExpense ?? default;
        ExpenseTitle = expense?.Description ?? string.Empty;

        _isAssignTimePropertyChange = false;
    }
    private bool _isAssignTimePropertyChange;

    public void EditExpenseDetails()
    {
        if (_isAssignTimePropertyChange)
        {
            return;
        }
    
        var editExpenseRequestObj = new EditExpenseRequest(CancellationToken.None, new ExpenseDetailedViewPresenterCallBack(this), DateOfExpense.DateTime, ExpenseTitle, ExpenseNote, _expense, Store.CurrentUserBobj);

        var editExpenseUseCase = InstanceBuilder.CreateInstance<EditExpense>(editExpenseRequestObj);

        editExpenseUseCase.Execute();
    }


    public void ExpenseObjLoaded(ExpenseVobj expenseObj)
    {
        if (expenseObj is null)
        {
            return;
        }
        _expense = expenseObj;

        SplittrNotification.CurrencyPreferenceChanged += SplittrNotification_CurrencyPreferenceChanged;

        SplittrNotification.ExpenseEdited += SplittrNotification_ExpenseEdited;

        GetGroupName(expenseObj);

        CallRelatedExpenseUseCase();

        PopulatePropertyData(_expense);
    }

    private void SplittrNotification_ExpenseEdited(ExpenseEditedEventArgs obj)
    {
        if (obj?.EditedExpenseObj?.Equals(_expense) is true)
        {
            var editedExpense = obj.EditedExpenseObj;
            _expense.DateOfExpense = editedExpense.DateOfExpense;
            _expense.Description = editedExpense.Description;
            _expense.Note = editedExpense.Note;
        }
    }

    private string FormatExpenseOwnerInfo(ExpenseVobj expense)
    {
        var ownerName = IsCurrentUserRaisedExpense(expense) ? "You" : expense?.SplitRaisedOwner.UserName;
        var dateOfExpense = expense?.CreatedDate.ToString("MMMM dd yyyy");

        return $"Added By {ownerName} on {dateOfExpense}";
    }
    private bool IsCurrentUserRaisedExpense(ExpenseVobj expense)
    {
        return expense.SplitRaisedOwner?.Equals(Store.CurrentUserBobj) is true;
    }

    private void SplittrNotification_CurrencyPreferenceChanged(CurrencyPreferenceChangedEventArgs obj)
    {
        CalculateTotalExpenditureBeforeSplit(RelatedExpenses);

    }

    public void ViewDisposed()
    {
        SplittrNotification.CurrencyPreferenceChanged -= SplittrNotification_CurrencyPreferenceChanged;

        SplittrNotification.ExpenseEdited -= SplittrNotification_ExpenseEdited;
    }

    private void GetGroupName(ExpenseVobj expenseObj)
    {
        if (expenseObj?.GroupUniqueId == null)
        {
            return;
        }
        var getGroupDetail = new GroupDetailByIdRequest(expenseObj.GroupUniqueId,
            CancellationToken.None,
            new ExpenseDetailedViewPresenterCallBack(this),
            Store.CurrentUserBobj);

        var getGroupDetailUseCaseObj = InstanceBuilder.CreateInstance<GroupDetailById>(getGroupDetail);

        getGroupDetailUseCaseObj.Execute();
    }




    private void CallRelatedExpenseUseCase()
    {
        var cts = new CancellationTokenSource();

        var relatedExpenseRequestObj = new RelatedExpenseRequestObj(_expense, Store.CurrentUserBobj, cts.Token, new ExpenseDetailedViewPresenterCallBack(this));

        var relatedExpenseUseCase = InstanceBuilder.CreateInstance<RelatedExpense>(relatedExpenseRequestObj);

        relatedExpenseUseCase.Execute();
    }

    private void CalculateTotalExpenditureBeforeSplit(IEnumerable<ExpenseBobj> relatedExpenses)
    {
        if (!relatedExpenses.Any()) // Total Expense Calculation if No Related Expenses 
        {
            return;
        }

        //Total Amount Before Split
        var expenditureAmountBeforeSplit = relatedExpenses.Where(relatedExpense => !relatedExpense.ExpenseUniqueId.Equals(_expense.ExpenseUniqueId)).Sum(relatedExpense => relatedExpense.StrExpenseAmount) + _expense.StrExpenseAmount;

        _ = RunOnUiThread(() =>
        {
            TotalExpenditureAmount = expenditureAmountBeforeSplit;

        }).ConfigureAwait(false);

    }


    private void PopulateRelatedExpenses(IEnumerable<ExpenseBobj> relatedExpenses)
    {
        //sum of all expenses before split 
        CalculateTotalExpenditureBeforeSplit(relatedExpenses);

        RelatedExpenses.Clear();

        RelatedExpenses.Add(_expense);

        var expenseVMobjs = relatedExpenses.Select(ex => new ExpenseVobj(ex));

        RelatedExpenses.AddRange(expenseVMobjs);

    }

    public void CategoryChangeSelected(ExpenseCategory expenseCategory)
    {
        if (_expense?.CategoryId != expenseCategory.ParentCategoryId)
        {
            var categoryChangeRequestObj = new ChangeExpenseCategoryReq(CancellationToken.None, new ExpenseDetailedViewPresenterCallBack(this), _expense, expenseCategory, Store.CurrentUserBobj);
            var categoryChangeUseCase = InstanceBuilder.CreateInstance<ChangeExpenseCategory>(categoryChangeRequestObj);
            categoryChangeUseCase.Execute();
        }
    }
    private class ExpenseDetailedViewPresenterCallBack : IPresenterCallBack<RelatedExpenseResponseObj>, IPresenterCallBack<GroupDetailByIdResponse>, IPresenterCallBack<ChangeExpenseCategoryResponse>, IPresenterCallBack<EditExpenseResponse>
    {
        private readonly ExpenseDetailedViewUserControlViewModel _viewModel;
        public ExpenseDetailedViewPresenterCallBack(ExpenseDetailedViewUserControlViewModel viewModel)
        {
            _viewModel = viewModel;

        }
        public void OnSuccess(RelatedExpenseResponseObj result)
        {
            _ = RunOnUiThread(() =>
            {
                _viewModel.PopulateRelatedExpenses(result.RelatedExpenses);

            }).ConfigureAwait(false);
        }
        public void OnSuccess(GroupDetailByIdResponse result)
        {
            if (result?.RequestedGroup != null)
            {
                _ = RunOnUiThread(() =>
                {
                    _viewModel.ExpenseOccuredGroupName = result.RequestedGroup.GroupName;

                }).ConfigureAwait(false);
            }
        }
        public void OnSuccess(ChangeExpenseCategoryResponse result)
        {
            if (_viewModel._expense?.ExpenseUniqueId.Equals(result.UpdatedExpenseBobj.ExpenseUniqueId) is true)
            {
                _viewModel._expense.IconSource = result.ChangedExpenseCategory.Icon;
                _viewModel._expense.CategoryId = result.ChangedExpenseCategory.Id;
                _viewModel._expense.CategoryName = result.ChangedExpenseCategory.Name;
            }
        }
        void IPresenterCallBack<EditExpenseResponse>.OnSuccess(EditExpenseResponse result)
        {
            _ = RunOnUiThread((() =>
            {
                if (result?.EditedExpenseObj is ExpenseVobj editedExpense)
                {
                    _viewModel.PopulatePropertyData(editedExpense);
                    return;
                }
                if (result?.EditedExpenseObj is not null)
                {
                    _viewModel.PopulatePropertyData(new ExpenseVobj(result.EditedExpenseObj));
                }
            }));
        }
        void IPresenterCallBack<EditExpenseResponse>.OnError(SplittrException ex)
        {
            _ = RunOnUiThread((() =>
            {
                _viewModel.PopulatePropertyData(_viewModel._expense);
                
            }));
            ExceptionHandlerService.HandleException(ex);
        }
        public void OnError(SplittrException ex)
        {
            if (ex.InnerException is SqlException)
            {
                // Ui Notify to retry 
            }
        }
    }

}