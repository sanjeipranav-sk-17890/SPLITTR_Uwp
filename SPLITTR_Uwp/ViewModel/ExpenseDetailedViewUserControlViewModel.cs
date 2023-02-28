using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetGroupDetails;
using SPLITTR_Uwp.Core.UseCase.GetRelatedExpense;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;
using static SPLITTR_Uwp.Services.UiService;

namespace SPLITTR_Uwp.ViewModel;

internal class ExpenseDetailedViewUserControlViewModel :ObservableObject
{
        
    private double _totalExpenditureAmount;

    public ObservableCollection<ExpenseVobj> RelatedExpenses { get; } = new ObservableCollection<ExpenseVobj>();

    public double TotalExpenditureAmount
    {
        get => _totalExpenditureAmount;
        set => SetProperty(ref _totalExpenditureAmount, value);
    }

    //Storing Reference Of passed Expense ,utilized to make Ui manupulation
    private ExpenseVobj _expense;

    private string _expenseOccuredGroupName = string.Empty;

    public void ExpenseObjLoaded(ExpenseVobj expenseObj)
    {
        if (expenseObj is null)
        {
            return;
        }
        _expense = expenseObj;

        SplittrNotification.CurrencyPreferenceChanged += SplittrNotification_CurrencyPreferenceChanged;

        GetGroupName(expenseObj);

        CallRelatedExpenseUseCase();
    }

    private  void SplittrNotification_CurrencyPreferenceChanged(CurrencyPreferenceChangedEventArgs obj)
    {
          
        CalculateTotalExpenditureBeforeSplit(RelatedExpenses);
         
    }

    public void ViewDisposed()
    {
        SplittrNotification.CurrencyPreferenceChanged -= SplittrNotification_CurrencyPreferenceChanged;
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

    public string ExpenseOccuredGroupName
    {
        get => _expenseOccuredGroupName;
        set => SetProperty(ref _expenseOccuredGroupName, value);
    }


    private void CallRelatedExpenseUseCase()
    {
        var cts = new CancellationTokenSource();

        var relatedExpenseRequestObj = new RelatedExpenseRequestObj(_expense, Store.CurrentUserBobj, cts.Token, new ExpenseDetailedViewPresenterCallBack(this));

        var relatedExpenseUseCase = InstanceBuilder.CreateInstance<RelatedExpense>(relatedExpenseRequestObj);

        relatedExpenseUseCase.Execute();
    }

    private async void CalculateTotalExpenditureBeforeSplit(IEnumerable<ExpenseBobj> relatedExpenses)
    {
        if (!relatedExpenses.Any()) // Total Expense Calculation if No Related Expenses 
        {
            return;
        }

        //Total Amount Before Split
        var expenditureAmountBeforeSplit = relatedExpenses.Where(relatedExpense => !relatedExpense.ExpenseUniqueId.Equals(_expense.ExpenseUniqueId)).Sum(relatedExpense => relatedExpense.StrExpenseAmount) + _expense.StrExpenseAmount;

        await RunOnUiThread(() =>
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

    private class ExpenseDetailedViewPresenterCallBack : IPresenterCallBack<RelatedExpenseResponseObj>,IPresenterCallBack<GroupDetailByIdResponse>
    {
        private readonly ExpenseDetailedViewUserControlViewModel _viewModel;
        public ExpenseDetailedViewPresenterCallBack(ExpenseDetailedViewUserControlViewModel viewModel)
        {
            _viewModel = viewModel;

        }
        public async void OnSuccess(RelatedExpenseResponseObj result)
        {
            await RunOnUiThread(() =>
            {
                _viewModel.PopulateRelatedExpenses(result.RelatedExpenses);

            }).ConfigureAwait(false);
        }
        public async void OnSuccess(GroupDetailByIdResponse result)
        {
            if (result?.RequestedGroup != null)
            {
                await  RunOnUiThread(() =>
                {
                    _viewModel.ExpenseOccuredGroupName = result.RequestedGroup.GroupName;

                }).ConfigureAwait(false);
            }
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