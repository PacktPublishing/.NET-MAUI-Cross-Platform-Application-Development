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
            CheckFingerPrintStatus();
        }

        [RelayCommand(CanExecute = nameof(ValidateLogin))]
        private async void Login()
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
                    Login();
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
            return !String.IsNullOrWhiteSpace(Username) && IsFingerprintEnabled;
        }

        private async void CheckFingerPrintStatus()
        {
            IsFingerprintEnabled = await _fingerprint.IsAvailableAsync();
        }

        [ObservableProperty]
        private bool isFingerprintEnabled = false;

        [ObservableProperty]
        private bool isBusy = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        [NotifyCanExecuteChangedFor(nameof(FingerprintLoginCommand))]
        private string? username = default;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string? password = default;

        public List<string> GetUsersList()
        {
            return User.GetUsersList();
        }

    }
}
