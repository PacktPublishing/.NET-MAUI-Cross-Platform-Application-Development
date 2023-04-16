using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

using PassXYZ.Vault.Properties;

namespace PassXYZ.Vault.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? title = Properties.Resources.About;

        [RelayCommand]
        private async Task OpenWeb()
        {
            await Browser.OpenAsync("Properties.Resources.about_url");
        }

        public string GetStoreName()
        {
            return "Test Database";
        }

        public DateTime GetStoreModifiedTime()
        {
            return DateTime.Now;
        }

    }
}