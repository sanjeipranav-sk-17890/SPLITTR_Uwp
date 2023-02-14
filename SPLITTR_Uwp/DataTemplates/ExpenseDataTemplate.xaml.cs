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
using SPLITTR_Uwp.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates
{
    public sealed partial class ExpenseDataTemplate : UserControl
    {
       
        
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
             ExpenseAmountTextBlock.Text = GetFormatedExpenseAmount();
             CurrencySymbolTextBlock.Text = ExpenseObj?.StrExpenseAmount.ExpenseSymbol(Store.CurreUserBobj) ?? string.Empty;
             ExpenseItemTitleTextBox.Text = GetFormatedTitle(ExpenseObj);
             ExpensePersonProfileInnerRectangle.Fill = GetRespectiveLogo(ExpenseObj);
             AssignExpenseStatus(ExpenseObj);
        }

        private void AssignExpenseStatus(ExpenseViewModel expenseObj)
        {
            if (expenseObj is null)
            {
                return;
            }
            ExpenseStatusTextBlock.Text = ExpenseObj.ExpenseStatus.ToString();
           var  expenseStatusForeground = ExpenseObj.ExpenseStatus switch
            {
                Core.ModelBobj.Enum.ExpenseStatus.Cancelled => CancelledColorBrush,
                Core.ModelBobj.Enum.ExpenseStatus.Paid => PaidColorBrush,
                _ => PendingColorBrush
            };
            AssignExpenseStatusForeGround(expenseStatusForeground);

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



        private void AssignExpenseStatusForeGround(Brush expenseStatusIndicatorBrush)
        {
            ExpenseStatusTextBlock.Foreground = expenseStatusIndicatorBrush;
            ExpenseStatusIndicatorIcon.Foreground = expenseStatusIndicatorBrush;
        }

        #endregion

        #region UI Binding & Dependency Property

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

        private async void ExpenseObj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await UiService.RunOnUiThread((() =>
            {
                LoadValuesInUi();

                Bindings.Update();
            }));
            
        }


    }
}
