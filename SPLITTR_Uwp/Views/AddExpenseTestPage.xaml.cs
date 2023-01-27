using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using SPLITTR_Uwp.Services;
using Windows.UI.WindowManagement.Preview;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SPLITTR_Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddExpenseTestPage : Page
    {
        public AddExpenseTestPage()
        {
            this.InitializeComponent();
            StateService.OnUserLoggedOut += StateService_OnUserLoggedOut;
        }

        private  void StateService_OnUserLoggedOut(Core.ModelBobj.UserBobj obj)
        {
            if (_sptrAppView is null)
            {
                return;
            }
            _splittrCoreWindow.Dispatcher?.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await  _sptrAppView.TryConsolidateAsync();
            });
        }

        private static ApplicationView _sptrAppView = null;
        private static CoreApplicationView _splittrCoreWindow = null;


        private Grid _rootGrid = null;
      

        private async void SplitExpenseControl_OnDragStarting(UIElement sender, DragStartingEventArgs args)
        {
            if (_sptrAppView is not null)
            {
                return;
            } 
            _splittrCoreWindow = CoreApplication.CreateNewView();

            await _splittrCoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (() =>
            {
                //Assign separated ctrl to New Window root Element
                _rootGrid = new Grid();
                var splitCntrl = new SplitExpenseUserControl();
                _rootGrid.Children.Add(splitCntrl);

                //Registering Root Xaml Element To ThemeHelper onLoaded 
                _rootGrid.Loaded += (o, eventArgs) =>
                {
                    //Applying Current Theme To Root Grid
                    _rootGrid.RequestedTheme = ThemeHelperService.GetPreferenceThemeIfSet();
                    ThemeHelperService.RegisterElement(_rootGrid);
                    //  SplitExpenseUCtrl.XamlRoot = _rootGrid.XamlRoot;
                };

                Window.Current.Content = _rootGrid;

                Window.Current.Activate();

                _sptrAppView = ApplicationView.GetForCurrentView();

                _sptrAppView.Consolidated += SptrAppView_Consolidated;
                
            }));
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(_sptrAppView.Id);
        }

        private void SptrAppView_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            _sptrAppView.Consolidated -= SptrAppView_Consolidated;
            ThemeHelperService.UnRegisterElement(_rootGrid);
            _splittrCoreWindow = null;
            _sptrAppView = null;
            _rootGrid = null;
        }

    }
}
