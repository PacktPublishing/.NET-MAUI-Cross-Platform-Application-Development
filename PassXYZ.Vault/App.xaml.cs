using PassXYZ.Vault.Services;
using PassXYZ.Vault.Views;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Application = Microsoft.Maui.Controls.Application;

namespace PassXYZ.Vault
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
			Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));

			DependencyService.Register<MockDataStore>();
			MainPage = new AppShell();
		}
		private async void OnMenuItemClicked(System.Object sender, System.EventArgs e)
		{
			await Shell.Current.GoToAsync("//LoginPage");
		}
	}
}
