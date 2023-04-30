using PassXYZ.Vault.Services;
using PassXYZ.Vault.Views;

namespace PassXYZ.Vault;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}
}
