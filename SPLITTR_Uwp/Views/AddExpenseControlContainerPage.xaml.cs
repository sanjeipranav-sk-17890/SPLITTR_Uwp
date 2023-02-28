using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Services;

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
            InitializeComponent();
            StateService.OnUserLoggedOut += StateService_OnUserLoggedOut;
        }

        private  void StateService_OnUserLoggedOut(UserBobj obj)
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

        private static ApplicationView _sptrAppView;
        private static CoreApplicationView _splittrCoreWindow;


        private Grid _rootGrid;
      

        private async void SplitExpenseControl_OnDragStarting(UIElement sender, DragStartingEventArgs args)
        {
            if (_sptrAppView is not null)
            {
                return;
            } 
            _splittrCoreWindow = CoreApplication.CreateNewView();

            await _splittrCoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //Assign separated ctrl to New Window root Element
                _rootGrid = new Grid();
                var splitCntrl = new SplitExpenseUserControl();
                _rootGrid.Children.Add(splitCntrl);

                //Registering Root Xaml Element To ThemeHelper onLoaded 
                _rootGrid.Loaded += OnRootGridOnLoaded;

                Window.Current.Content = _rootGrid;

                Window.Current.Activate();

                _sptrAppView = ApplicationView.GetForCurrentView();

                //Registering For System Accent Color Change
                AccentColorService.Register(_splittrCoreWindow.CoreWindow);

                _sptrAppView.Consolidated += SptrAppView_Consolidated;
                
            });

            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(_sptrAppView.Id);

            MainPage.RequestMainPageNavigation();
        }

        private void OnRootGridOnLoaded(object o, RoutedEventArgs eventArgs)
        {
            //Applying Current Theme To Root Grid
            _rootGrid.RequestedTheme = ThemeHelperService.GetPreferenceThemeIfSet();
            ThemeHelperService.RegisterElement(_rootGrid);

            //Applying Background To Root Element
            _rootGrid.Background = (Brush)Application.Current.Resources["ApplicationMainThemeWindowAcrylicBrush"];

        }

        private void SptrAppView_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            
            _sptrAppView.Consolidated -= SptrAppView_Consolidated;
            //Unsubscribe For Theme Transition
            ThemeHelperService.UnRegisterElement(_rootGrid);
            //Unsubscribe For Accent Color Transition
            AccentColorService.UnRegister(_splittrCoreWindow.CoreWindow);
            //Releasing Resources
            _splittrCoreWindow = null;
            _sptrAppView = null;
            _rootGrid = null;
        }

    }
}
