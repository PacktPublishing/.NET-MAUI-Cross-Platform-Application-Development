using System.Collections.ObjectModel;
using PassXYZLib;
using PassXYZ.Vault.ViewModels;
using System.Diagnostics;

namespace PassXYZ.Vault.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel _viewModel;

        private List<string> _users => User.GetUsersList();
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
            if (_users != null && _users.Count > 1)
            {
                switchUsersButton.IsVisible = true;
            }
        }

        private async void OnSwitchUsersClicked(object sender, EventArgs e)
        {
            if (_users != null)
            {
                var username = await DisplayActionSheet(Properties.Resources.pt_id_switchusers, Properties.Resources.action_id_cancel, null, _users.ToArray());
                if (username != Properties.Resources.action_id_cancel)
                {
                    messageLabel.Text = "";
                    _viewModel.Username = usernameEntry.Text = username;
                    _viewModel.Password = passwordEntry.Text = "";
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.Logout();
            passwordEntry.Text = "";
            _viewModel.CheckFingerprintStatus();
        }
    }
}