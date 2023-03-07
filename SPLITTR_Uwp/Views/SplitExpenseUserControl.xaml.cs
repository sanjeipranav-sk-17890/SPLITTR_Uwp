using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using SPLITTR_Uwp.Core.Models;
using SPLITTR_Uwp.ViewModel;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.Views;

public sealed partial class SplitExpenseUserControl : UserControl,ISplitExpenseView
{
    private readonly SplitExpenseViewModel _viewModel;
    public SplitExpenseUserControl()
    {
        _viewModel = ActivatorUtilities.CreateInstance<SplitExpenseViewModel>(App.Container,this);
        InitializeComponent();
        DataContext = _viewModel;
        Loaded += SplitExpenseUserControl_Loaded;
    }

    private void SplitExpenseUserControl_Loaded(object sender, RoutedEventArgs e)
    {
        Bindings.Update();
    }

    public XamlRoot VisualRoot
    {
        get => XamlRoot.Content.XamlRoot;
    }


    private void ExpenseCategoryControl_OnOnExpenseCategorySelected(ExpenseCategory category)
    {
        if (category is null)
        {
            return;
        }
        ExpenseCategoryControl.CategoryIconSource = new BitmapImage(new Uri(category.Icon));
        _viewModel.PreferedExpenseCategoryChanged(category);
    }
}



public interface ISplitExpenseView
{
    //if App Window used Xaml Root 
    public XamlRoot VisualRoot { get;}

    public CoreDispatcher Dispatcher { get;}

}