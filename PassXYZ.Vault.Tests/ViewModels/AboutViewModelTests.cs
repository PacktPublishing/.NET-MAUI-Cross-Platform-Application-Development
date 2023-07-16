using KPCLib;
using Microsoft.Extensions.Logging;

using PassXYZ.Vault.Services;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Tests.ViewModels
{
    public class AboutViewModelTests
    {
        ILogger<AboutViewModel> aboutViewModelLogger;
        readonly IDataStore<Item> dataStore;
        UserService userService;

        public AboutViewModelTests()
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                builder.AddDebug()
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug));
            var logger = loggerFactory.CreateLogger<UserService>();
            aboutViewModelLogger = loggerFactory.CreateLogger<AboutViewModel>();
            dataStore = new DataStore();
            userService = new UserService(dataStore, logger);
        }

        [Fact]
        public void SetTitleTest()
        {
            // TODO: Need to add .NET target in PassXYZLib, PxUser is not implemented in .NET target.
            //string AboutTitle = "AboutTest";
            //var loginservice = new LoginService(userService);
            //AboutViewModel viewModel = new(loginservice, aboutViewModelLogger);
            //viewModel.Title = AboutTitle;
            //Assert.Equal(AboutTitle, viewModel.Title);
        }
    }
}
