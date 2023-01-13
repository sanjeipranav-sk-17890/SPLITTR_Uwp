using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.ViewModel.Models;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.ViewModel;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates
{
    public sealed partial class RelatedExpenseTemplate : UserControl
    {

        private readonly RelatedExpenseTemplateViewModel _viewModel;

        private ExpenseViewModel ExpenseObj
        {
            get => DataContext as ExpenseViewModel;
        }

        public RelatedExpenseTemplate()
        {
            _viewModel = ActivatorUtilities.GetServiceOrCreateInstance<RelatedExpenseTemplateViewModel>(App.Container);
            this.InitializeComponent();
            DataContextChanged += RelatedExpenseTemplate_DataContextChanged;
            
            
        }

        private void RelatedExpenseTemplate_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (ExpenseObj is null)
            {
                return;
            }
            ExpenseObj.PropertyChanged += ExpenseObj_PropertyChanged;
            _viewModel.DataContextLoaded(ExpenseObj);
            Bindings.Update();
            AssignExpenseStatusForeGround();
        }

        private void ExpenseObj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AssignExpenseStatusForeGround();
            Bindings.Update();
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
