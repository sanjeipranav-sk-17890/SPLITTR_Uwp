using System;
using System.Diagnostics;
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

        private async void StateService_OnUserLoggedOut(Core.ModelBobj.UserBobj obj)
        {
            //if (_sptrAppWindow is null || !_sptrAppWindow.IsVisible)
            //{
            //    return;
            //}
            //await _sptrAppWindow.CloseAsync();
        }

        private ApplicationView _sptrAppView = null;

        private SplitExpenseUserControl SplitExpenseUCtrl { get; set; }

        private Grid _rootGrid = null;

        private SplitExpenseUserControl SeparateExpenseSplitControl()
        {
            var splitExpenseUCtrl = MainGrid.FindName(nameof(SplitExpneseControl)) as SplitExpenseUserControl;
            MainGrid.Children.Remove(splitExpenseUCtrl);
            if (splitExpenseUCtrl is not null)
            {
                splitExpenseUCtrl.CanDrag = false;
            }
            return splitExpenseUCtrl;
        }


        //private async void SplitExpenseControl_OnDragStarting(UIElement sender, DragStartingEventArgs args)
        //{
        //    //Removing UserControl Form Grid and Storing The Reference of it
        //    SplitExpenseUCtrl = SeparateExpenseSplitControl();

        //   if (_sptrAppView is null)
        //   {
        //       _sptrAppView = await AppWindow.TryCreateAsync();

        //       //Assign separated ctrl to New Window root Element
        //       _rootGrid = new Grid();
        //       _rootGrid.Children.Add(SplitExpenseUCtrl);

        //        //Registering Root Xaml Element To ThemeHelper onLoaded 
        //        _rootGrid.Loaded += (o, eventArgs) =>
        //        {
        //            //Applying Current Theme To Root Grid
        //            _rootGrid.RequestedTheme = ThemeHelperService.GetPreferenceThemeIfSet();
        //            ThemeHelperService.RegisterElement(_rootGrid);
        //            SplitExpenseUCtrl.XamlRoot = _rootGrid.XamlRoot;
        //        };

        //        SetMinimumWindowWidth(_sptrAppView,new Size(500,700));

        //        //Setting Window Content Property to root Grid
        //        ElementCompositionPreview.SetAppWindowContent(_sptrAppView, _rootGrid);
        //       _sptrAppView.RequestMoveAdjacentToCurrentView();

        //       _sptrAppView.Closed += SptrAppWindowClosed;

        //   }
        //   await _sptrAppView.TryShowAsync();

        //}
        private async void SplitExpenseControl_OnDragStarting(UIElement sender, DragStartingEventArgs args)
        {
            //Removing UserControl Form Grid and Storing The Reference of it
            SplitExpenseUCtrl = SeparateExpenseSplitControl();

            if (_sptrAppView is null)
            {
                var newView = CoreApplication.CreateNewView();


               await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (() =>
                {
                    //Assign separated ctrl to New Window root Element
                    _rootGrid = new Grid();
                    var splitCntrl = new SplitExpenseUserControl(SplitExpenseUCtrl);
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

                   _sptrAppView.Consolidated += _sptrAppView_Consolidated;

                }));

               await ApplicationViewSwitcher.TryShowAsStandaloneAsync(_sptrAppView.Id);



               // SetMinimumWindowWidth(_sptrAppView, new Size(500, 700));

               //Setting Window Content Property to root Grid
               //  ElementCompositionPreview.SetAppWindowContent(_sptrAppView, _rootGrid);
               //  _sptrAppView.RequestMoveAdjacentToCurrentView();

               //   _sptrAppView.Closed += SptrAppWindowClosed;

            }
             //  await _sptrAppView.TryShowAsync();

        }

        private void _sptrAppView_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            _sptrAppView.Consolidated -= _sptrAppView_Consolidated;
            ThemeHelperService.UnRegisterElement(_rootGrid);
            _sptrAppView = null;
            _rootGrid = null;
        }

        private void SetMinimumWindowWidth(AppWindow window, Size minimumWindowWidth)
        {
            // If specified size is smaller than the default min size for a window we need to set a new preferred min size first.
            // Let's set it to the smallest allowed and leave it at that.
           // WindowManagementPreview.SetPreferredMinSize(_sptrAppView, new Size(800, 1000));

            // Request the size of our window
           // _sptrAppView.RequestSize(new Size(500, 700));
        }
        private void SptrAppWindowClosed(AppWindow sender, AppWindowClosedEventArgs args)
        {
          //  _sptrAppView.Closed -= SptrAppWindowClosed;
            _sptrAppView = null;
            _rootGrid.Children.Remove(SplitExpenseUCtrl);
            SplitExpenseUCtrl.CanDrag = true;
            ThemeHelperService.UnRegisterElement(_rootGrid);
            _rootGrid = null;
            MainGrid.Children.Add(SplitExpenseUCtrl);
        }
    }
}
