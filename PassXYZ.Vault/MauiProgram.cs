using Microsoft.Extensions.Logging;
using KPCLib;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.Views;
using PassXYZ.Vault.ViewModels;
using User = PassXYZLib.User;

namespace PassXYZ.Vault;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("fa-regular-400.ttf", "FontAwesomeRegular");
				fonts.AddFont("fa-solid-900.ttf", "FontAwesomeSolid");
				fonts.AddFont("fa-brands-400.ttf", "FontAwesomeBrands");
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
			});

#if DEBUG
		builder.Logging.AddDebug();
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif
        builder.Services.AddSingleton<IDataStore<Item>, MockDataStore>();
        builder.Services.AddSingleton<IUserService<User>, UserService>();
        builder.Services.AddSingleton<LoginService>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<ItemDetailViewModel>();
        builder.Services.AddSingleton<ItemDetailPage>();
        builder.Services.AddSingleton<NewItemViewModel>();
        builder.Services.AddSingleton<NewItemPage>();
        builder.Services.AddSingleton<AboutViewModel>();
        builder.Services.AddSingleton<AboutPage>();
        builder.Services.AddTransient<ItemsViewModel>();
        builder.Services.AddTransient<ItemsPage>();

        return builder.Build();
	}
}
