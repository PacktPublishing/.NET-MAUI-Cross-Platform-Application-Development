using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;

using PassXYZ.Vault.Properties;

namespace PassXYZ.Vault.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = Properties.Resources.About;
            OpenWebCommand = new Command(async () => await Browser.OpenAsync(Properties.Resources.about_url));
        }

        public ICommand OpenWebCommand { get; }

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