using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

using PassXYZLib;

using PassXYZ.Vault.Services;
using PassXYZ.Vault.ViewModels;
using Microsoft.Extensions.Logging;

namespace PassXYZ.Vault.Views;

public partial class SettingsPage : ContentPage
{
    private readonly LoginService _currentUser;
    private readonly ILogger<LoginViewModel> _logger;
    private readonly LoginViewModel _viewModel;

    public SettingsPage(LoginViewModel viewModel, LoginService user, ILogger<LoginViewModel> logger)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _currentUser = user;
        _logger = logger;
        Title = Properties.Resources.menu_id_settings;
    }

    private void SetFingerprintSwitcher()
    {
        FingerprintSwitcher.IsEnabled = _viewModel.IsFingerprintAvailable;
        FingerprintSwitcher.On = _viewModel.IsFingerprintEnabled;
        if (_viewModel.IsFingerprintAvailable)
        {
            FingerprintSwitcher.Text = Properties.Resources.settings_fingerprint_remark;
        }
        else
        {
            FingerprintSwitcher.Text = Properties.Resources.settings_fingerprint_disabled;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        timerCell.Text = Properties.Resources.settings_timer_title + " " + PxUser.AppTimeout.ToString() + " " + Properties.Resources.settings_timer_unit_seconds;

        try
        {
            _viewModel.CheckFingerprintStatus();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex}");
        }
        SetFingerprintSwitcher();
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


    private async void SetResultAsync(bool result)
    {
        if (result)
        {
            try
            {
                await _currentUser.SetSecurityAsync(_currentUser.Password);
                _viewModel.IsFingerprintEnabled = true;
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
                _logger.LogError("SettingsPage: in SetResultAsync, {ex}", ex);
            }
        }
        else
        {
            FingerprintSwitcher.Text = "Turn on fingerprint error.";
        }
        SetFingerprintSwitcher();
    }

    private async void OnSwitcherToggledAsync(object sender, ToggledEventArgs e)
    {
        if (!_viewModel.IsFingerprintAvailable) { return; }

        if (e.Value)
        {
            try
            {
                string data = await _currentUser.GetSecurityAsync();
                if (data == null)
                {
                    var status = await _viewModel.AuthenticateAsync(Properties.Resources.fingerprint_login_message);
                    SetResultAsync(status);
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