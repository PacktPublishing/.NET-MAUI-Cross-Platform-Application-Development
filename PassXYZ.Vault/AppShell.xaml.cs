using System.Diagnostics;

using PassXYZ.Vault.Views;
using PassXYZ.Vault.Services;

namespace PassXYZ.Vault;

public partial class AppShell : Shell
{
    public static AppShell? CurrentAppShell { get; private set; } = default!;

    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
        //Routing.RegisterRoute(nameof(NotesPage), typeof(NotesPage));
        Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        //Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        //Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
        Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
        CurrentAppShell = this;
    }

    /// <summary>
    /// Logout
    /// </summary>
    private async void OnMenuItemClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("AppShell: Logout");
        LoginUser.Instance.Logout();

        await Current.GoToAsync("//LoginPage");
    }

    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        if (args.Current != null)
        {
            Debug.WriteLine($"AppShell: source={args.Current.Location}, target={args.Target.Location}");
        }
    }

    public void SetRootPageTitle(string name)
    {
        RootItem.Title = name;
        RootItem.FlyoutItemIsVisible = true;
    }
}
