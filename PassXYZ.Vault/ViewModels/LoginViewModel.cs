using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

using PassXYZLib;
using PassXYZ.Vault.Views;
using PassXYZ.Vault.Services;
using System.Diagnostics;

namespace PassXYZ.Vault.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private LoginService _currentUser;
        ILogger<LoginViewModel> _logger;

        public LoginViewModel(LoginService user, ILogger<LoginViewModel> logger)
        { 
            _currentUser = user;
            _logger = logger;
        }

        [RelayCommand(CanExecute = nameof(ValidateLogin))]
        private async Task Login(object obj)
        {
            try
            {
                IsBusy = true;

                if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Username))
                {
                    await Shell.Current.DisplayAlert("", Properties.Resources.settings_empty_password, Properties.Resources.alert_id_ok);
                    IsBusy = false;
                    return;
                }

                _currentUser.Username = Username;
                _currentUser.Password = Password;
                bool status = await _currentUser.LoginAsync();

                if (status)
                {
                    if (AppShell.CurrentAppShell != null)
                    {
                        AppShell.CurrentAppShell.SetRootPageTitle(Username);

                        await Shell.Current.GoToAsync($"//RootPage");
                    }
                    else
                    {
                        throw (new NullReferenceException("CurrentAppShell is null"));
                    }
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                string msg = ex.Message;
                if (ex is System.IO.IOException ioException)
                {
                    _logger.LogError("Login error, need to recover");
                    msg = Properties.Resources.message_id_recover_datafile;
                }
                await Shell.Current.DisplayAlert(Properties.Resources.LoginErrorMessage, msg, Properties.Resources.alert_id_ok);
            }
        }

        private bool ValidateLogin()
        {
            var canExecute = !String.IsNullOrWhiteSpace(Username)
                && !String.IsNullOrWhiteSpace(Password);
            return canExecute;
        }

        [RelayCommand(CanExecute = nameof(ValidateSignUp))]
        private async Task SignUp()
        {
            try
            {
                IsBusy = true;

                if (string.IsNullOrWhiteSpace(Password2) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Username))
                {
                    await Shell.Current.DisplayAlert("", Properties.Resources.settings_empty_password, Properties.Resources.alert_id_ok);
                    IsBusy = false;
                    return;
                }

                _currentUser.Username = Username;
                _currentUser.Password = Password;

                if (_currentUser.IsUserExist)
                {
                    await Shell.Current.DisplayAlert(Properties.Resources.SignUpPageTitle, Properties.Resources.SignUpErrorMessage1, Properties.Resources.alert_id_ok);
                    IsBusy = false;
                    return;
                }

                await _currentUser.SignUpAsync();
                IsBusy = false;
                _ = await Shell.Current.Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(Properties.Resources.SignUpPageTitle, ex.Message, Properties.Resources.alert_id_ok);
            }
            Debug.WriteLine($"LoginViewModel: OnSignUpClicked {_currentUser.Username}, DeviceLock: {_currentUser.IsDeviceLockEnabled}");
        }

        private bool ValidateSignUp()
        {
            var canExecute = !String.IsNullOrWhiteSpace(Username)
                && !String.IsNullOrWhiteSpace(Password)
                && !String.IsNullOrWhiteSpace(Password2);
            
            if (canExecute)
            {
                return Password!.Equals(Password2);
            }

            return canExecute;
        }

        [ObservableProperty]
        private bool isBusy = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private string? username = default;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private string? password = default;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignUpCommand))]
        private string? password2 = default;

        public bool IsDeviceLockEnabled
        {
            get
            {
                return _currentUser.IsDeviceLockEnabled;
            }

            set
            {
                _currentUser.IsDeviceLockEnabled = value;
            }
        }

        public List<string> GetUsersList()
        {
            return User.GetUsersList();
        }

        public void Logout() 
        {
            _currentUser.Logout();
        }
    }
}
