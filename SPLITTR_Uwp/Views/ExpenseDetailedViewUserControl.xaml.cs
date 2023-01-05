using Windows.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.ViewModel;
using SPLITTR_Uwp.ViewModel.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.Views
{
    public sealed partial class ExpenseDetailedViewUserControl : UserControl
    {
        public ExpenseViewModel ExpenseObj
        {
            get => DataContext as ExpenseViewModel;
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
            ManipulateUiBasedOnDataContext();

            //UnderGoing VM logic to Fetch Related Expenses 
            _viewModel.ExpenseObjLoaded(ExpenseObj);
        }
        private void ManipulateUiBasedOnDataContext()
        {
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
    }
}
