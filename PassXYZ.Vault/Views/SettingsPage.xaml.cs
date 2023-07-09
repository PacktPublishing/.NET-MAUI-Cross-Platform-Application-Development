using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

using PassXYZLib;

using PassXYZ.Vault.Services;
using PassXYZ.Vault.ViewModels;
using Microsoft.Extensions.Logging;

namespace PassXYZ.Vault.Views;

public partial class SettingsPage : ContentPage
{
    private LoginService _currentUser;
    ILogger<LoginViewModel> _logger;
    private readonly LoginViewModel _viewModel;
    FingerprintAvailability _availability = FingerprintAvailability.NoFingerprint;

    public SettingsPage(LoginViewModel viewModel, LoginService user, ILogger<LoginViewModel> logger)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _currentUser = user;
        _logger = logger;
        Title = Properties.Resources.menu_id_settings;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _availability = await CrossFingerprint.Current.GetAvailabilityAsync();

        timerCell.Text = Properties.Resources.settings_timer_title + " " + PxUser.AppTimeout.ToString() + " " + Properties.Resources.settings_timer_unit_seconds;

        // Refresh username and device lock status
        _ = _viewModel.Username;
        if (_availability == FingerprintAvailability.Available)
        {
            FingerPrintSwitcher.IsEnabled = true;
            FingerPrintSwitcher.On = true;

            try
            {
                string data = await _currentUser.GetSecurityAsync();
                FingerPrintSwitcher.On = data != null;
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
                FingerPrintSwitcher.IsEnabled = false;
                FingerPrintSwitcher.On = false;
                FingerPrintSwitcher.Text = Properties.Resources.settings_fingerprint_disabled;
                _logger.LogError($"{ex}");
            }
        }
        else
        {
            FingerPrintSwitcher.IsEnabled = false;
            //FingerPrintSwitcher.IsVisible = false;
            FingerPrintSwitcher.Text = Properties.Resources.settings_fingerprint_disabled;
        }
    }

    private async void OnTimerTappedAsync(object sender, System.EventArgs e)
    {
        List<string> timerlist = new List<string>();
        var timer_30seconds = "30 " + Properties.Resources.settings_timer_unit_seconds;
        timerlist.Add(timer_30seconds);
        var timer_2minutes = "2 " + Properties.Resources.settings_timer_unit_minutes;
        timerlist.Add(timer_2minutes);
        var timer_5minutes = "5 " + Properties.Resources.settings_timer_unit_minutes;
        timerlist.Add(timer_5minutes);
        var timer_10minutes = "10 " + Properties.Resources.settings_timer_unit_minutes;
        timerlist.Add(timer_10minutes);
        var timer_30minutes = "30 " + Properties.Resources.settings_timer_unit_minutes;
        timerlist.Add(timer_30minutes);
        var timer_1hour = "1 " + Properties.Resources.settings_timer_unit_hour;
        timerlist.Add(timer_1hour);

        var timerValue = await DisplayActionSheet(Properties.Resources.settings_timer_title, Properties.Resources.action_id_cancel, null, timerlist.ToArray());
        if (timerValue == timer_30seconds) { PxUser.AppTimeout = 30; }
        else if (timerValue == timer_2minutes) { PxUser.AppTimeout = 120; }
        else if (timerValue == timer_5minutes) { PxUser.AppTimeout = 300; }
        else if (timerValue == timer_10minutes) { PxUser.AppTimeout = 600; }
        else if (timerValue == timer_30minutes) { PxUser.AppTimeout = 1800; }
        else if (timerValue == timer_1hour) { PxUser.AppTimeout = 3600; }

        timerCell.Text = Properties.Resources.settings_timer_title + " " + PxUser.AppTimeout.ToString() + " " + Properties.Resources.settings_timer_unit_seconds;
    }

    private async Task AuthenticateAsync(string reason, string? cancel = null, string? fallback = null, string? tooFast = null)
    {
            CancellationTokenSource cancelToken;

        cancelToken = new CancellationTokenSource();

        var dialogConfig = new AuthenticationRequestConfiguration("Verify your fingerprint", reason)
        { // all optional
            CancelTitle = cancel,
            FallbackTitle = fallback,
            AllowAlternativeAuthentication = false
        };

        // optional
        dialogConfig.HelpTexts.MovedTooFast = tooFast;

        var result = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig, cancelToken.Token);

        SetResultAsync(result);
    }

    private async void SetResultAsync(FingerprintAuthenticationResult result)
    {
        if (result.Authenticated)
        {
            try
            {
                await _currentUser.SetSecurityAsync(_currentUser.Password);
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
                _logger.LogError("SettingsPage: in SetResultAsync, {ex}", ex);
            }
        }
        else
        {
            FingerPrintSwitcher.Text = $"{result.Status}: {result.ErrorMessage}";
        }
    }

    private async void OnSwitcherToggledAsync(object sender, ToggledEventArgs e)
    {
        if (_availability == FingerprintAvailability.NoFingerprint) { return; }

        if (e.Value)
        {
            try
            {
                string data = await _currentUser.GetSecurityAsync();
                if (data == null)
                {
                    await AuthenticateAsync(Properties.Resources.fingerprint_login_message);
                }
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
                _logger.LogError("{ex}", ex);
            }
        }
        else
        {
            // Turn off fingerprint
            _ = await _currentUser.DisableSecurityAsync();
        }
    }
}