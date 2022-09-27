using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

using PassXYZ.Vault.ViewModels;
using PassXYZ.Vault.Properties;

namespace PassXYZ.Vault.Views
{
    public partial class AboutPage : ContentPage
    {
        AboutViewModel viewModel;
        public AboutPage()
        {
            InitializeComponent();
            viewModel = new AboutViewModel();
            BindingContext = viewModel;
            DatabaseName.Text = viewModel.GetStoreName();

            DateTime localTime = viewModel.GetStoreModifiedTime().ToLocalTime();
            LastModifiedDate.Text = localTime.ToLongDateString();
            LastModifiedTime.Text = localTime.ToLongTimeString();

            var assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var version = Properties.Resources.Version + " " + ((assemblyVersion != null) ? assemblyVersion.ToString() : "");
#if DEBUG
            version = version + " (Debug)";
#endif
            AppVersion.Text = version;
            Debug.WriteLine($"Version: {version}");
        }
    }
}