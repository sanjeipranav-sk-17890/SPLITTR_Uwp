using System.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.DataManager.Contracts;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Utility;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;

namespace SPLITTR_Uwp.ViewModel
{
    internal class RelatedExpenseTemplateViewModel : ObservableObject
    {
        
        private readonly IStringManipulator _stringManipulator;
        private readonly IExpenseHistoryUsecase _expenseHistory;
        private bool _isExpenseMarkedAsPaid;

        public RelatedExpenseTemplateViewModel(IStringManipulator stringManipulator,IExpenseHistoryUsecase expenseHistory)
        {
         
            _stringManipulator = stringManipulator;
            _expenseHistory = expenseHistory;

        }
        public bool IsParentComment
        {
            get => ExpenseObj?.ParentExpenseId is null;
        }

        public string CurrencySymbol
        {
            get => ExpenseObj is null ? string.Empty : Store.CurreUserBobj.StrWalletBalance.ExpenseSymbol(Store.CurreUserBobj); // Fetching Currency symbol Corresponding to user preference
        }

        public string UserInitial
        {
            get => ExpenseObj is null ? string.Empty : _stringManipulator.GetUserInitial(ExpenseObj.CorrespondingUserObj.UserName);
        }

        public string ExpenditureAmount
        {
            get => ExpenseObj is null ? string.Empty : ExpenseObj.StrExpenseAmount.ToString("##.0000");
        }

        public string FormatedUserName
        {
            get
            {
                if (ExpenseObj is null)
                {
                    return string.Empty;
                }
                return ExpenseObj.CorrespondingUserObj.Equals(Store.CurreUserBobj) ? "you" : ExpenseObj.CorrespondingUserObj.UserName;
            }
        }

        private ExpenseViewModel ExpenseObj { get; set; }

        public bool IsExpenseMarkedAsPaid
        {
            get => _isExpenseMarkedAsPaid;
            set => SetProperty(ref _isExpenseMarkedAsPaid, value);
        }

        public void DataContextLoaded(ExpenseViewModel expense)
        {
            if (expense == null) { return;}
            ExpenseObj = expense;
            ExpenseObj.PropertyChanged += ExpenseObj_PropertyChanged;

            CheckExpenseMarkHistory();
        }
        private void CheckExpenseMarkHistory()
        {
            //Call to Database To check whether it is marked as take place only if it is a Paid
            if (ExpenseObj.ExpenseStatus != ExpenseStatus.Paid)
            {
                IsExpenseMarkedAsPaid = false;
                return;
            }
            _expenseHistory.IsExpenseMarkedAsPaid(ExpenseObj.ExpenseUniqueId, async isPaid =>
            {
               await UiService.RunOnUiThread(() =>
               {
                   IsExpenseMarkedAsPaid = isPaid;
                   Debug.WriteLine($"{ExpenseObj.ExpenseUniqueId}------,{isPaid}-------------{ExpenseObj.CorrespondingUserObj.UserName}");
               });

            } );
        }


        private void ExpenseObj_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           CheckExpenseMarkHistory();   
        }
    }
}
