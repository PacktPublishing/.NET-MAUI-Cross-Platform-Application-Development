using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;

using PassXYZ.Vault.Properties;
using PassXYZ.Vault.Services;

namespace PassXYZ.Vault.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        ILogger<AboutViewModel> _logger;
        private LoginService _currentUser;

        public AboutViewModel(LoginService user, ILogger<AboutViewModel> logger)
        {
            _currentUser = user;
            _logger = logger;
        }

        [ObservableProperty]
        private string? title = Properties.Resources.About;

        [RelayCommand]
        private async Task OpenWeb()
        {
            await Browser.OpenAsync(Properties.Resources.about_url);
        }

        public string GetStoreName()
        {
            return _currentUser.Username;
        }

        public DateTime GetStoreModifiedTime()
        {
            return DateTime.Now;
        }

    }
}