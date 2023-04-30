using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Diagnostics;
using System.Reflection;

using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.Views;

namespace PassXYZ.Vault;

public partial class App : Application
{
    public static bool InBackground = false;
    private static bool _isLogout = false;

    /// <summary>
    /// When a connection is timeout, the network is not stable.
    /// We will try to connect again when the app resume or restart.
    /// </summary>
    public static bool IsSshOperationTimeout { get; set; } = false;

    public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}

    protected override void OnStart()
    {
        InBackground = false;
        IsSshOperationTimeout = false;
        InitTestDb();
        ExtractIcons();
    }

    protected override void OnSleep()
    {
        // Handle when your app sleeps
        InBackground = true;

        // Lock screen after timeout
        Device.StartTimer(TimeSpan.FromSeconds(PxUser.AppTimeout), () =>
        {
            if (InBackground)
            {

                // TODO: dataStore.Logout();
                _isLogout = true;
                return false;
            }
            else
            {
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
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("//LoginPage");
            });
            _isLogout = false;
        }

    }

    private void ExtractIcons()
    {
        var assembly = this.GetType().GetTypeInfo().Assembly;
        foreach (EmbeddedDatabase iconFile in EmbeddedIcons.IconFiles)
        {
            if (!File.Exists(iconFile.Path))
            {
                using (var stream = assembly.GetManifestResourceStream(iconFile.ResourcePath))
                using (var fileStream = new FileStream(iconFile.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        if (!File.Exists(EmbeddedIcons.iconZipFile.Path))
        {
            using (var stream = assembly.GetManifestResourceStream(EmbeddedIcons.iconZipFile.ResourcePath))
            using (var fileStream = new FileStream(EmbeddedIcons.iconZipFile.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                stream.CopyTo(fileStream);
            }
            ZipFile.ExtractToDirectory(EmbeddedIcons.iconZipFile.Path, PxDataFile.IconFilePath);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    private void InitTestDb()
    {
        foreach (EmbeddedDatabase eDb in TEST_DB.DataFiles)
        {
            if (!File.Exists(eDb.Path))
            {
                var assembly = this.GetType().GetTypeInfo().Assembly;
                using (var stream = assembly.GetManifestResourceStream(eDb.ResourcePath))
                using (var fileStream = new FileStream(eDb.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }
    }
}
