using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.DataHandler.Contracts;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Utility;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;
using static System.Net.Mime.MediaTypeNames;

namespace SPLITTR_Uwp.ViewModel
{
    internal class RelatedExpenseTemplateViewModel : ObservableObject
    {
        private readonly DataStore _store;
        private readonly IStringManipulator _stringManipulator;
        private readonly IExpenseHistoryUsecase _expenseHistory;
        private bool _isExpenseMarkedAsPaid;

        public RelatedExpenseTemplateViewModel(DataStore store,IStringManipulator stringManipulator,IExpenseHistoryUsecase expenseHistory)
        {
            _store = store;
            _stringManipulator = stringManipulator;
            _expenseHistory = expenseHistory;

        }
        public bool IsParentComment
        {
            get => ExpenseObj?.ParentExpenseId is null;
        }

        public string CurrencySymbol
        {
            get => ExpenseObj is null ? string.Empty : _store.UserBobj.StrWalletBalance.ExpenseSymbol(_store.UserBobj); // Fetching Currency symbol Corresponding to user preference
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
                return ExpenseObj.CorrespondingUserObj.Equals(_store.UserBobj) ? "you" : ExpenseObj.CorrespondingUserObj.UserName;
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
                return;
            }
            _expenseHistory.IsExpenseMarkedAsPaid(ExpenseObj.ExpenseUniqueId, async isPaid =>
            {
               await UiService.RunOnUiThread(() =>
               {
                   IsExpenseMarkedAsPaid = isPaid;
               });

            } );
        }


        private void ExpenseObj_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           CheckExpenseMarkHistory();   
        }
    }
}
