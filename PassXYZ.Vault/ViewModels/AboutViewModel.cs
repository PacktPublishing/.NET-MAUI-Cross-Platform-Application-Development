using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace PassXYZ.Vault.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? title = "About";

        [RelayCommand]
        private async Task OpenWeb()
        {
            await Browser.OpenAsync("https://learn.microsoft.com/en-us/dotnet/maui/?view=net-maui-7.0");
        }
    }
}