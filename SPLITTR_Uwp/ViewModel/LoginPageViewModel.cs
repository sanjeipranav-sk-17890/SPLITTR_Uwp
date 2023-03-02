using System;
using System.Text.RegularExpressions;
using System.Threading;
using Windows.UI.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using SPLITTR_Uwp.Core.SplittrExceptions;
using SPLITTR_Uwp.Core.UseCase;
using SPLITTR_Uwp.Core.UseCase.LoginUser;
using SPLITTR_Uwp.DataRepository;
using SPLITTR_Uwp.Services;
using SPLITTR_Uwp.Views;

namespace SPLITTR_Uwp.ViewModel;

public class LoginPageViewModel : ObservableObject
{
    private readonly IStateService _stateManager;
    private int _selectedItem;
    private bool _loginInformation;
    private bool _wrongUserCredentialVisibility;

    public int SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }


    public bool LoginInformation
    {
        get => _loginInformation;
        set => SetProperty(ref _loginInformation, value);
    }

    public bool WrongUserCredentialVisibility
    {
        get => _wrongUserCredentialVisibility;
        set => SetProperty(ref _wrongUserCredentialVisibility, value);
    }

    public string LoginUserEmail { get; set; }

    private readonly DispatcherTimer _timer;

    public LoginPageViewModel(IStateService stateManager)
    {
        _stateManager = stateManager;
        //Firing a event for every 0.5 seconds
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2)
        };
        _timer.Tick += Timer_Tick;
        _timer.Start();

    }

    private int _change = 1;

    private void Timer_Tick(object sender, object e)
    {
        SelectedItem += _change;
        if (SelectedItem < 1 || SelectedItem > 2)
        {
            _change *= -1;
        }
    }
    private bool ValidateEmail(string email)
    {
        var regex = new Regex("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$");
        return regex.IsMatch(email);
    }

    public  void LoginButtonPressed()
    {

        if (string.IsNullOrWhiteSpace(LoginUserEmail))
        {
            LoginInformation = true;
            WrongUserCredentialVisibility = false;
            return;
        }
        if (!ValidateEmail(LoginUserEmail))
        {
            LoginInformation = false;
            WrongUserCredentialVisibility = true;
        }
        else
        {
                
            var loginReqObj = new LoginRequestObj(LoginUserEmail.Trim().ToLower(), new LoginVmPresenterCalBack(this), CancellationToken.None);

            var loginUseCaseObj = InstanceBuilder.CreateInstance<UserLogin>(loginReqObj);

            loginUseCaseObj.Execute();
        }


    }
    
    public void PageUnloaded()
    {
        //stopping main page animation if the the page is unloaded
        _timer.Stop();
    }
    private async void OnLoginCompleted(LoginResponseObj result)
    {
        await UiService.RunOnUiThread(() =>
        {
            if (!result.IsUserAlreadyExist)
            {
                WrongUserCredentialVisibility = true;
                return;
            }
            WrongUserCredentialVisibility = false;
            Store.CurrentUserBobj = result.LoginUserCred;
            NavigationService.Navigate<MainPage>();
        });
        //Storing Current User Session 
        _stateManager?.RegisterUserLoginSession(Store.CurrentUserBobj);
    }


    private class LoginVmPresenterCalBack : IPresenterCallBack<LoginResponseObj>
    {
        private readonly LoginPageViewModel _viewModel;
        public LoginVmPresenterCalBack(LoginPageViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        public void OnSuccess(LoginResponseObj result)
        {
            _viewModel.OnLoginCompleted(result);
        }
        public async void OnError(SplittrException ex)
        {
            await UiService.RunOnUiThread(() =>
            {
                if (ex.InnerException is ArgumentNullException)
                {
                    _viewModel.WrongUserCredentialVisibility = true;
                }
            });

        }
    }
}