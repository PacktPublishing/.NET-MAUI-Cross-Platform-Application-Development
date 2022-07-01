using Microsoft.AspNetCore.Components;
using System.Diagnostics;

using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.ViewModels;
using Microsoft.AspNetCore.Components.Web;

namespace PassXYZ.Vault.Pages;

public partial class Login : ComponentBase
{
    [Inject]
    private IUserService<User> userService { get; set; } = default!;
    [Inject]
    private IDataStore<Item> dataStore { get; set; } = default!;
    [Inject]
    private NavigationManager navigationManager { get; set; } = default!;
    private LoginUser currentUser => LoginUser.Instance;

    private async void OnLogin(MouseEventArgs e)
    {
        Debug.WriteLine($"username={currentUser.Username}, password={currentUser.Password}");
        bool status = await userService.LoginAsync(currentUser);
        if (status)
        {
            string path = Path.Combine(PxDataFile.TmpFilePath, currentUser.FileName);
            if (File.Exists(path))
            {
                // If there is file to merge, we merge it first.
                bool result = await dataStore.MergeAsync(path);
            }
            navigationManager.NavigateTo("/items");
        }
    }
}