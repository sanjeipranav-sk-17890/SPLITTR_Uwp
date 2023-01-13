using Windows.UI.Xaml.Controls;
using SPLITTR_Uwp.ViewModel;
using Microsoft.Extensions.DependencyInjection;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.Views
{
    public sealed partial class SplitExpenseUserControl : UserControl
    {
        private readonly SplitExpenseViewModel _viewModel;
        public SplitExpenseUserControl()
        {
            _viewModel = ActivatorUtilities.CreateInstance<SplitExpenseViewModel>(App.Container);
            this.InitializeComponent();
            this.DataContext = _viewModel;
           
        }



        
    }
}
