using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.EventArg;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.DataManager;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.GetRelatedExpense;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;

namespace SPLITTR_Uwp.ViewModel
{
    
    internal class ExpenseDetailedViewUserControlViewModel :ObservableObject
    {
        
        private double _totalExpenditureAmount;

        public ObservableCollection<ExpenseViewModel> RelatedExpenses { get; } = new ObservableCollection<ExpenseViewModel>();

        public double TotalExpenditureAmount
        {
            get => _totalExpenditureAmount;
            set => SetProperty(ref _totalExpenditureAmount, value);
        }

        //Storing Reference Of passed Expense ,utilized to make Ui manupulation
        private ExpenseViewModel _expense;
        public void ExpenseObjLoaded(ExpenseViewModel expenseObj)
        {
            if (expenseObj is null)
            {
                return;
            }
            _expense = expenseObj;

            var cts = new CancellationTokenSource();
            var relatedExpenseRequestObj = new RelatedExpenseRequestObj(expenseObj, Store.CurreUserBobj, cts.Token, new ExpenseDetailedViewPresenterCallBack(this));

            var relatedExpenseUseCase = InstanceBuilder.CreateInstance<RelatedExpense>(relatedExpenseRequestObj);

            relatedExpenseUseCase.Execute();

        }
      
      
        public void PopulateRelatedExpenses(IEnumerable<ExpenseBobj> relatedExpenses)
        {
            //sum of all expenses before split 
            TotalExpenditureAmount = relatedExpenses.Sum(relatedExpense => relatedExpense.StrExpenseAmount) + _expense.StrExpenseAmount;

            RelatedExpenses.Clear();

            RelatedExpenses.Add(_expense);

            var expenseVMobjs = relatedExpenses.Select(ex => new ExpenseViewModel(ex));

            RelatedExpenses.AddRange(expenseVMobjs);

        }


        public string FetchGroupName(string groupUniqueId)
        {
            if (groupUniqueId is null)
            {
                return string.Empty;
            }
            var groupName=string.Empty;
            foreach (var userGroup in Store.CurreUserBobj.Groups)
            {
                if (groupUniqueId.Equals(userGroup.GroupUniqueId))
                {
                  groupName= userGroup.GroupName;
                }
                
            }
            return groupName;
        }
        private class ExpenseDetailedViewPresenterCallBack : IPresenterCallBack<RelatedExpenseResponseObj>
        {
            private readonly ExpenseDetailedViewUserControlViewModel _viewModel;
            public ExpenseDetailedViewPresenterCallBack(ExpenseDetailedViewUserControlViewModel viewModel)
            {
                _viewModel = viewModel;

            }
            public async void OnSuccess(RelatedExpenseResponseObj result)
            {
                await UiService.RunOnUiThread(() =>
                {
                    _viewModel.PopulateRelatedExpenses(result.RelatedExpenses);

                }).ConfigureAwait(false);
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
}
