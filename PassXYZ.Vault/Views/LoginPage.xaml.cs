using System.Diagnostics;

using PassXYZLib;

using PassXYZ.Vault.Properties;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage()
    {
        InitializeComponent();
        BindingContext = _viewModel = new LoginViewModel();
    }
}