using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates
{
    public sealed partial class PaymentWindowExpenseView : UserControl
    {
        private readonly PaymentWindowExpenseViewModel _viewModel;

        public event Action<bool> SettleUpButtonClicked;

        private ExpenseVobj ExpenseObj
        {
            get => DataContext as ExpenseVobj;
        }

        public PaymentWindowExpenseView()
        {
            this.InitializeComponent();
            _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<PaymentWindowExpenseViewModel>(App.Container);
            DataContextChanged += PaymentWindowExpenseView_DataContextChanged;
        }


        private void PaymentWindowExpenseView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (ExpenseObj is null)
            {
                return;
            }
            Bindings.Update();
            _viewModel.ExpenseObjLoaded(ExpenseObj);
        }
        private void RadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            SettlePaymentButton.IsEnabled = true;
        }

        private void SettlePaymentButton_OnClick(object sender, RoutedEventArgs e)
        {
            SettleUpButtonClicked?.Invoke(WalletPaymentRadioButton.IsChecked?? false);
        }

    }
    internal class PaymentWindowExpenseViewModel : ObservableObject
    {
       
        private string _currentUserInitial;
        private string _owingUserInitial;
        private string _currencySymbol;

        public string CurrentUserInitial
        {
            get => _currentUserInitial;
            set => SetProperty(ref _currentUserInitial, value);
        }

        public bool IsWalletPayment { get; set; }
        public string OwingUserInitial
        {
            get => _owingUserInitial;
            set => SetProperty(ref _owingUserInitial, value);
        }

        public string CurrencySymbol
        {
            get => _currencySymbol;
            set => SetProperty(ref _currencySymbol, value);
        }

        public UserVobj CurrentUser { get; set; }

        public void ExpenseObjLoaded(ExpenseVobj expenseObj)
        {
            if (expenseObj is null)
            {
                return;
            }

            InitializeValues(expenseObj);
        }
        public PaymentWindowExpenseViewModel()
        {
            
            CurrentUser = new UserVobj(Store.CurreUserBobj);

        }
        private void InitializeValues(ExpenseVobj expenseObj)
        {
            CurrentUserInitial = expenseObj.CorrespondingUserObj.UserName.GetUserInitial();
            OwingUserInitial = expenseObj.SplitRaisedOwner.UserName.GetUserInitial();
            CurrencySymbol = expenseObj.StrExpenseAmount.ExpenseSymbol(Store.CurreUserBobj);
        }
    }
}
