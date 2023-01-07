using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.ConversationalAgent;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.Utility;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.Core.ModelBobj.Enum;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates
{
    public sealed partial class RelatedExpenseTemplate : UserControl,INotifyPropertyChanged
    {
        private  static IStringManipulator _stringManipulator;
        private static DataStore _store;

        private ExpenseViewModel ExpenseObj
        {
            get => DataContext as ExpenseViewModel;
        }

        public bool IsParentComment
        {
            get => ExpenseObj?.ParentExpenseId is  null;
        }




        public RelatedExpenseTemplate()
        {
            _stringManipulator ??= ActivatorUtilities.GetServiceOrCreateInstance<IStringManipulator>(App.Container);
            _store ??= ActivatorUtilities.GetServiceOrCreateInstance<DataStore>(App.Container);
            
            this.InitializeComponent();

            DataContextChanged += RelatedExpenseTemplate_DataContextChanged;
        }

        private void RelatedExpenseTemplate_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Bindings.Update();
            if (ExpenseObj is null)
            {
                return;
            }
            ExpenseObj.ValueChanged += ExpenseObj_ValueChanged;
            InitializeControlsWithValues();
        }

        private void ExpenseObj_ValueChanged()
        {
            InitializeControlsWithValues();
        }

        private void InitializeControlsWithValues()
        {
               PersonPicture.Initials = _stringManipulator.GetUserInitial(ExpenseObj.CorrespondingUserObj.UserName);
               CurrencySymbolTextBlock.Text = _store.UserBobj.StrWalletBalance.ExpenseSymbol(_store.UserBobj); // Fetching Currency symbol Corresponding to user preference
               CurrencyAmountTextBlock.Text = ExpenseObj.StrExpenseAmount.ToString("##.0000");
               UserNameTextBlock.Text = FormatUserName();
               AssignExpenseStatusForeGround();
        }


        private string FormatUserName()
        {
            return ExpenseObj.CorrespondingUserObj.Equals(_store.UserBobj) ? "you" : ExpenseObj.CorrespondingUserObj.UserName;
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

        private void AssignExpenseStatusForeGround()
        {
            ExpenseStatusBrush.Color = ExpenseObj.ExpenseStatus switch
            {
                ExpenseStatus.Pending => Windows.UI.Colors.DarkRed,
                ExpenseStatus.Cancelled => Windows.UI.Colors.Orange,
                _ => Windows.UI.Colors.DarkGreen
            };

        }
    }
}
