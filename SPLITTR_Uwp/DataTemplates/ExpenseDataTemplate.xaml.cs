using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates
{
    public sealed partial class ExpenseDataTemplate : UserControl, INotifyPropertyChanged
    {
        private string _expenseItemTitle;
        private Brush _expenseStatusForeground;
        private string _expenseStatus;
        
        private static ExpenseItemViewModel ViewModel { get; set; }

        public ExpenseViewModel ExpenseObj
        {
            get => this.DataContext as ExpenseViewModel;
        }


        #region UiBehaviuorLogic
        //if Converted expense amount more than 7 decimals it concat it and
        //Color the text Block green if the Owner is Current user Else text block is Red
        private string GetFormatedExpenseAmount()
        {
            if (ExpenseObj is null)
            {
                return string.Empty;
            }

            if (ExpenseObj.SplitRaisedOwner.Equals(Store.CurreUserBobj))
            {
                ExpenseAmountTextBlock.Foreground = PaidColorBrush;
                return ViewModel.FormatExpenseAmount(ExpenseObj);
            }
            ExpenseAmountTextBlock.Foreground = PendingColorBrush;
            return ViewModel.FormatExpenseAmount(ExpenseObj);
        }

        private string FormatExpenseObjDescription()
        {
            if (ExpenseObj is null)
            {
                return string.Empty;
            }

            if (ExpenseObj.Description?.Length > 10)
            {
                return ExpenseObj.Description?.Substring(0, 10) + " ....";
            }
            return ExpenseObj.Description ?? string.Empty;
        }

        public void LoadValuesInUi()
        {
             ExpenseItemTitle = GetFormatedTitle(ExpenseObj);
             ExpensePersonProfileInnerRectangle.Fill = GetRespectiveLogo(ExpenseObj);
             AssignExpenseStatus(ExpenseObj);
        }

        private void AssignExpenseStatus(ExpenseViewModel expenseObj)
        {
            if (expenseObj is null)
            {
                return;
            }
            ExpenseStatus = ExpenseObj.ExpenseStatus.ToString();
            ExpenseStatusForeground = ExpenseObj.ExpenseStatus switch
            {
                Core.ModelBobj.Enum.ExpenseStatus.Cancelled => CancelledColorBrush,
                Core.ModelBobj.Enum.ExpenseStatus.Paid => PaidColorBrush,
                _ => PendingColorBrush
            };

        }

        private Brush GetRespectiveLogo(ExpenseViewModel expenseObj)
        {
            if (expenseObj is null)
            {
                return IndividaulExpenseLogo;
            }
            //assinging respective image brush based on type of expense
            Brush brush = expenseObj.GroupUniqueId switch
            {
                not null => GroupExpenseLogo,
                _ => IndividaulExpenseLogo
            };
            return brush;
        }

        //Assigning Item title based on Expense type
        private string GetFormatedTitle(ExpenseViewModel expenseObj)
        {
            if (expenseObj is null)
            {
                return String.Empty;
            }
            if (expenseObj.GroupUniqueId is not null)
            {
                ExpenseHeadingTypeTextBox.Text = "Group :";
            }
            ExpenseHeadingTypeTextBox.Text = "Owner :";

            return ViewModel.FormatExpenseTitle(expenseObj);

        }





        #endregion

        #region UI Binding & Dependency Property
        public Brush ExpenseStatusForeground
        {
            get => _expenseStatusForeground;
            set => SetField(ref _expenseStatusForeground, value);
        }

        public string ExpenseItemTitle
        {
            get => _expenseItemTitle;
            set => SetField(ref _expenseItemTitle, value);
        }


        public string ExpenseStatus
        {
            get => _expenseStatus;
            set => SetField(ref _expenseStatus, value);
        }

        public string FormatedExpenseDescription
        {
            get => FormatExpenseObjDescription();
        }

        public string CurrencySymbol
        {
            get => ExpenseObj?.StrExpenseAmount.ExpenseSymbol(Store.CurreUserBobj);
        }

        public string FormatedExpenseAmount
        {
            get => GetFormatedExpenseAmount();

        }

        public readonly static DependencyProperty ExpenseStatusVisibilityProperty = DependencyProperty.Register(
            nameof(ExpenseStatusVisibility), typeof(Visibility), typeof(ExpenseDataTemplate), new PropertyMetadata(default(Visibility)));

        public Visibility ExpenseStatusVisibility
        {
            get => (Visibility)GetValue(ExpenseStatusVisibilityProperty);
            set => SetValue(ExpenseStatusVisibilityProperty, value);
        }

        public readonly static DependencyProperty DateOfExpenseTextBlockVisibilityProperty = DependencyProperty.Register(
            nameof(DateOfExpenseTextBlockVisibility), typeof(Visibility), typeof(ExpenseDataTemplate), new PropertyMetadata(default(Visibility)));

        public Visibility DateOfExpenseTextBlockVisibility
        {
            get => (Visibility)GetValue(DateOfExpenseTextBlockVisibilityProperty);
            set => SetValue(DateOfExpenseTextBlockVisibilityProperty, value);
        }



        #endregion

        public ExpenseDataTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += ExpenseDataTemplate_DataContextChanged;
            ViewModel ??= ActivatorUtilities.CreateInstance<ExpenseItemViewModel>(App.Container);
        }

        private void ExpenseDataTemplate_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Bindings.Update();


            if (ExpenseObj is null)
            {
                return;
            }
            ExpenseObj.PropertyChanged += ExpenseObj_PropertyChanged;
            LoadValuesInUi();
        }

        private void ExpenseObj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LoadValuesInUi();
            //OnPropertyChanged(nameof(FormatedExpenseDescription));
            //OnPropertyChanged(nameof(CurrencySymbol));
            //OnPropertyChanged(nameof(FormatedExpenseAmount));
            Bindings.Update();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void ExpenseDataTemplate_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AdditionalDataGrid.Visibility = Visibility.Visible;
        }
        private void ExpenseDataTemplate_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            AdditionalDataGrid.Visibility = Visibility.Collapsed;
        }
    }
}
