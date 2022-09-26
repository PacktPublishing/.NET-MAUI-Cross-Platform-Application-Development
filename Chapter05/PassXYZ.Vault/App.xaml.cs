using System.Collections.ObjectModel;
using System.Diagnostics;

using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.Views;

namespace PassXYZ.Vault;

public partial class App : Application
{
    public static bool InBackground = false;
    private static bool _isLogout = false;
    public static LoginUser CurrentUser
    {
        get
        {
            return LoginUser.Instance;
        }
    }

    /// <summary>
    /// When a connection is timeout, the network is not stable.
    /// We will try to connect again when the app resume or restart.
    /// </summary>
    public static bool IsSshOperationTimeout { get; set; } = false;

    public App()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
        Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
		Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));

		DependencyService.Register<MockDataStore>();
        DependencyService.Register<UserService>();
        MainPage = new AppShell();
	}

    protected override void OnStart()
    {
        InBackground = false;
        IsSshOperationTimeout = false;
        //InitTestDb();
        //ExtractIcons();

        Debug.WriteLine($"PassXYZ: OnStart, InBackground={InBackground}");
    }

    protected override void OnSleep()
    {
        // Handle when your app sleeps
        InBackground = true;
        Debug.WriteLine($"PassXYZ: OnSleep, InBackground={InBackground}");

        // Lock screen after timeout
        Device.StartTimer(TimeSpan.FromSeconds(PxUser.AppTimeout), () =>
        {
            if (InBackground)
            {

                // TODO: dataStore.Logout();
                _isLogout = true;
                Debug.WriteLine("PassXYZ: Timer, force logout.");
                return false;
            }
            else
            {
                Debug.WriteLine("PassXYZ: Timer, running in foreground.");
                return false;
            }
        });
    }

    protected override void OnResume()
    {
        InBackground = false;
        IsSshOperationTimeout = false;
        if (_isLogout)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("//LoginPage");
            });
            _isLogout = false;
            Debug.WriteLine("PassXYZ: OnResume, force logout");
        }

        Debug.WriteLine($"PassXYZ: OnResume, InBackground={InBackground}");
    }

}
