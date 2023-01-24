using System.Diagnostics;
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
            _viewModel.BindingUpdateInvoked += _viewModel_BindingUpdateInvoked;
            Unloaded += SplitExpenseUserControl_Unloaded;
            Loaded += SplitExpenseUserControl_Loaded;
        }

        private void SplitExpenseUserControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //SuggestionListTeachingTip.XamlRoot = XamlRoot;
            SuggestionListTeachingTip.Target = SplitUserNameTextBlock;
            Debug.WriteLine("loaded SplitExpenseasdfsdf");
        }

        private void SplitExpenseUserControl_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
          Debug.WriteLine("Unloaded SplitExpenseasdfsdf");
        }

        private void _viewModel_BindingUpdateInvoked()
        {
           Bindings.Update();
        }
    }
}
