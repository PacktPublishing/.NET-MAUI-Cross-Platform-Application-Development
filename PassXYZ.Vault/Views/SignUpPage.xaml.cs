using System.Diagnostics;

using PassXYZLib;

using PassXYZ.Vault.Properties;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SignUpPage : ContentPage
{
    LoginViewModel _viewModel;

    public SignUpPage()
    {
        InitializeComponent();
        BindingContext = _viewModel = new LoginViewModel();
    }

    public SignUpPage(Action<string> signUpAction)
    {
        InitializeComponent();
        BindingContext = _viewModel = new LoginViewModel(signUpAction);
    }
}