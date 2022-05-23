using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Services; 
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

        builder.Services.AddSingleton<IDataStore<Item>, DataStore>();
        builder.Services.AddSingleton<IUserService<User>, UserService>();
        builder.Services.AddSingleton<LoginUser, LoginUser>();

        return builder.Build();
	}
}
