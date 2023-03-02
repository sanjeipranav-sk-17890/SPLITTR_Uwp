using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.SplittrException;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.VerifyPaidExpense;
using SPLITTR_Uwp.Core.Utility;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.ViewModel.Models;
using SQLite;

namespace SPLITTR_Uwp.ViewModel;

internal class RelatedExpenseTemplateViewModel : ObservableObject
{
        
    private readonly IStringManipulator _stringManipulator;
    private bool _isExpenseMarkedAsPaid;

    public RelatedExpenseTemplateViewModel(IStringManipulator stringManipulator)
    {
         
        _stringManipulator = stringManipulator;
            
    }
    public bool IsParentComment
    {
        get => ExpenseObj?.ParentExpenseId is null;
    }

    public string CurrencySymbol
    {
        get => ExpenseObj is null ? string.Empty : Store.CurrentUserBobj.StrWalletBalance.ExpenseSymbol(Store.CurrentUserBobj); // Fetching Currency symbol Corresponding to user preference
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
            return ExpenseObj.CorrespondingUserObj.Equals(Store.CurrentUserBobj) ? "you" : ExpenseObj.CorrespondingUserObj.UserName;
        }
    }

    private ExpenseVobj ExpenseObj { get; set; }

    public bool IsExpenseMarkedAsPaid
    {
        get => _isExpenseMarkedAsPaid;
        set => SetProperty(ref _isExpenseMarkedAsPaid, value);
    }

    public void DataContextLoaded(ExpenseVobj expense)
    {
        if (expense == null)
        {
            return;
        }
        ExpenseObj = expense;
        ExpenseObj.PropertyChanged += ExpenseObj_PropertyChanged;
        SplittrNotification.CurrencyPreferenceChanged += SplittrNotification_CurrencyPreferenceChanged;
        CheckExpenseMarkHistory();
    }

    private async void SplittrNotification_CurrencyPreferenceChanged(CurrencyPreferenceChangedEventArgs obj)
    {
        await UiService.RunOnUiThread(() =>
        {
            OnPropertyChanged(nameof(ExpenditureAmount));
            OnPropertyChanged(nameof(CurrencySymbol));
        }).ConfigureAwait(false);
    }

    public void ViewDisposed()
    {
        SplittrNotification.CurrencyPreferenceChanged -= SplittrNotification_CurrencyPreferenceChanged;
    }
    private void CheckExpenseMarkHistory()
    {
        //Call to Database To check whether it is marked as take place only if it is a Paid
        if (ExpenseObj.ExpenseStatus != ExpenseStatus.Paid)
        {
            IsExpenseMarkedAsPaid = false;
            return;
        }
        var cts = new CancellationTokenSource();

        var verifyExpensePaidRequestObj = new VerifyPaidExpenseRequestObj(ExpenseObj.ExpenseUniqueId, cts.Token, new RelatedExpenseTemplateVmPresenterCallBack(this));

        var verifyExpenseUseCaseObj = InstanceBuilder.CreateInstance<VerifyPaidExpense>(verifyExpensePaidRequestObj);

        verifyExpenseUseCaseObj.Execute();
    }


    private void ExpenseObj_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        CheckExpenseMarkHistory();
    }
    private async void InvokeOnVerifyPaidExpenseCompleted(VerifyPaidExpenseResponseObj result)
    {
        await UiService.RunOnUiThread(() =>
        {
            IsExpenseMarkedAsPaid = result.IsExpenseMarkAsPaid;
            Debug.WriteLine($"{ExpenseObj.ExpenseUniqueId}------,{result.IsExpenseMarkAsPaid}-------------{ExpenseObj.CorrespondingUserObj.UserName}");
        }).ConfigureAwait(false);

    }
    private class RelatedExpenseTemplateVmPresenterCallBack : IPresenterCallBack<VerifyPaidExpenseResponseObj>
    {
        private readonly RelatedExpenseTemplateViewModel _viewModel;
        public RelatedExpenseTemplateVmPresenterCallBack(RelatedExpenseTemplateViewModel viewModel)
        {
            _viewModel = viewModel;

        }
        public void OnSuccess(VerifyPaidExpenseResponseObj result)
        {
            _viewModel.InvokeOnVerifyPaidExpenseCompleted(result);
        }
        public void OnError(SplittrException ex)
        {
            if (ex.InnerException is SQLiteException)
            {
                ExceptionHandlerService.HandleException(ex);
            }
        }
    }
}