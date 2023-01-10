using Windows.UI.Xaml;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.ViewModel;

internal class OwingMoneyPaymentExpenseViewModel
{
    public void PaymetForExpenseButtonClicked(Expense expense)
    {
        // C# code to create a new window
        var newWindow = WindowHelper.CreateWindow();
        var rootPage = new NavigationRootPage();
        rootPage.RequestedTheme = ThemeHelper.RootTheme;
        newWindow.Content = rootPage;
        newWindow.Activate();
    }

}
