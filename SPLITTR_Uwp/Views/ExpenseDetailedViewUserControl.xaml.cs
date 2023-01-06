using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.Views
{
    public sealed partial class ExpenseDetailedViewUserControl : UserControl,INotifyPropertyChanged
    {
        public ExpenseViewModel ExpenseObj
        {
            get => DataContext as ExpenseViewModel;
        }

        public string GroupName
        {
            get => GetGroupName();
        }

        private string GetGroupName()
        {
            if (ExpenseObj is null)
            {
                return string.Empty;
            }
            return _viewModel.FetchGroupName(ExpenseObj.GroupUniqueId);
        }

        private bool GroupNameVisibility
        {
            get => !string.IsNullOrEmpty(GroupName);
        }

        private bool IndividualSplitIconVisibility
        {
            get => !GroupNameVisibility;
        }

        private readonly ExpenseDetailedViewUserControlViewModel _viewModel;

        public ExpenseDetailedViewUserControl()
        {
            this.InitializeComponent();
            DataContextChanged += ExpenseDetailedViewUserControl_DataContextChanged;
            _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<ExpenseDetailedViewUserControlViewModel>(App.Container);
        }

        private void ExpenseDetailedViewUserControl_DataContextChanged(Windows.UI.Xaml.FrameworkElement sender, Windows.UI.Xaml.DataContextChangedEventArgs args)
        {
            Bindings.Update();


            if (ExpenseObj is null)
            {
                return;
            }
            ExpenseObj.ValueChanged += ExpenseObj_ValueChanged;


            ManipulateUiBasedOnDataContext();
        }

        private void ExpenseObj_ValueChanged()
        {
            ManipulateUiBasedOnDataContext();
        }

        private void ManipulateUiBasedOnDataContext()
        {

            //UnderGoing VM logic to Fetch Related Expenses 
            _viewModel.ExpenseObjLoaded(ExpenseObj);

            OnPropertyChanged(nameof(GroupName));
            OnPropertyChanged(nameof(GroupNameVisibility));
            OnPropertyChanged(nameof(IndividualSplitIconVisibility));
            AssignExpenseStatusForeGround();

        }
        private  void AssignExpenseStatusForeGround()
        {
            ExpenseStatusBrush.Color = ExpenseObj.ExpenseStatus switch
            {
                ExpenseStatus.Pending => Windows.UI.Colors.DarkRed,
                ExpenseStatus.Cancelled => Windows.UI.Colors.Orange,
                _ =>Windows.UI.Colors.DarkGreen
            };

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
    }
}
