using Microsoft.AspNetCore.Components;
using System.Diagnostics;

using PassXYZ.Vault.Services;
using PassXYZ.Vault.ViewModels;
using Microsoft.AspNetCore.Components.Web;

namespace PassXYZ.Vault.Pages;

public partial class Login : ComponentBase
{
    [Inject]
    LoginViewModel viewModel { get; set; } = default!;
    private LoginUser currentUser { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (currentUser == null)
        {
            currentUser = viewModel.CurrentUser;
        }
    }

    private void OnLogin(MouseEventArgs e)
    {
        Debug.WriteLine($"username={currentUser.Username}, password={currentUser.Password}");
        viewModel.OnLoginClicked();
    }
}