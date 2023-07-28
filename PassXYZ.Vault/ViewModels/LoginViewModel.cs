using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
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
        private readonly IFingerprint _fingerprint;

        public LoginViewModel(LoginService user, ILogger<LoginViewModel> logger, IFingerprint fingerprint)
        {
            _currentUser = user;
            _logger = logger;
            _fingerprint = fingerprint;
        }

        [RelayCommand(CanExecute = nameof(ValidateLogin))]
        private async Task Login()
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
                _logger.LogDebug("data path: {path}", _currentUser.Path);
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
            return canExecute;
        }

        [RelayCommand(CanExecute = nameof(ValidateFingerprintLogin))]
        private async Task FingerprintLogin()
        {
            var cancel = new CancellationTokenSource();
            var dialogConfig = new AuthenticationRequestConfiguration(Username,
                Properties.Resources.fingerprint_login_message)
            {
                CancelTitle = "Cancel fingerprint login",
                FallbackTitle = "Use Password",
                AllowAlternativeAuthentication = true,
            };

            var result = await _fingerprint.AuthenticateAsync(dialogConfig, cancel.Token);

            if (result.Authenticated)
            {
                // Username cannot be null when FingerprintLogin is invokved
                Password = await _currentUser.GetSecurityAsync();
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    await Login();
                }
                else
                {
                    _logger.LogWarning("GetSecurityAsync() error.");
                }
            }
            else
            {
                _logger.LogWarning("Failed to login with fingerprint.");
            }
        }

        private bool ValidateFingerprintLogin()
        {
            CheckFingerPrintStatus();
            return !String.IsNullOrWhiteSpace(Username);
        }

        public async void CheckFingerPrintStatus()
        {
            _currentUser.Username = Username;
            var password = await _currentUser.GetSecurityAsync();
            var isAvailable = await _fingerprint.IsAvailableAsync();
            IsFingerprintEnabled = isAvailable && !string.IsNullOrWhiteSpace(password);
        }

        [ObservableProperty]
        private bool isFingerprintEnabled = false;

        [ObservableProperty]
        private bool isBusy = false;

        private string? username = default;
        public string? Username
        {
            get => username;
            set
            {
                if (SetProperty(ref username, value))
                {
                    _currentUser.Username = value;
                    LoginCommand.NotifyCanExecuteChanged();
                    SignUpCommand.NotifyCanExecuteChanged();
                    FingerprintLoginCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private string? password = default;
        public string? Password
        {
            get => password;
            set
            {
                if (SetProperty(ref password, value))
                {
                    _currentUser.Password = value;
                    LoginCommand.NotifyCanExecuteChanged();
                    SignUpCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool CheckDeviceLock()
        {
            User user = new()
            {
                Username = this.Username,
            };

            if (user.IsUserExist)
            {
                // This is important, since we need to reset device lock status based on existing file.
                _currentUser.IsDeviceLockEnabled = user.IsDeviceLockEnabled;
                return !_currentUser.IsKeyFileExist && _currentUser.IsDeviceLockEnabled;
            }

            return false;
        }

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

    }
}
