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

        if (_viewModel.Users != null && _viewModel.Users.Count > 1)
        {
            switchUsersButton.IsVisible = true;
        }

        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            passwordEntry.ReturnType = ReturnType.Next;
        }
        else
        {
            passwordEntry.ReturnType = ReturnType.Done;
            passwordEntry.Completed += (object sender, EventArgs e) =>
            {
                _viewModel.OnLoginClicked();
            };
        }
    }

    private async void OnSwitchUsersClicked(object sender, EventArgs e)
    {
        if(_viewModel.Users != null)
        {
            var users = _viewModel.GetUsersList();
            var username = await DisplayActionSheet(Properties.Resources.pt_id_switchusers, Properties.Resources.action_id_cancel, null, users.ToArray());
            if (username != Properties.Resources.action_id_cancel)
            {
                messageLabel.Text = "";
                _viewModel.CurrentUser.Username = usernameEntry.Text = username;
                _viewModel.CurrentUser.Password = passwordEntry.Text = "";
                // InitFingerPrintButton();
            }
            Debug.WriteLine($"LoginPage: OnSwitchUsersClicked(Username: {_viewModel.CurrentUser.Username})");
        }
    }
}