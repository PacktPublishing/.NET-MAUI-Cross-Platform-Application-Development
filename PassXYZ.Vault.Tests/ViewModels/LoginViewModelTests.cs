using NSubstitute;
using Microsoft.Maui.Controls.Core.UnitTests;
using Microsoft.Extensions.Logging;

using KPCLib;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.ViewModels;
using Microsoft.Maui.Controls;

namespace PassXYZ.Vault.Tests.ViewModels
{
    public class LoginViewModelTests : ShellTestBase
    {
        Microsoft.Maui.Controls.Application app;
        ILogger<LoginViewModel> loginViewModelLogger;
        readonly IDataStore<Item> dataStore;
        UserService userService;

        public LoginViewModelTests() 
        {
            TestShell shell = new TestShell();
            var abougPage = new ShellItem { Route = "AboutPage" };
            var page = MakeSimpleShellSection("Readme", "content");
            abougPage.Items.Add(page);
            shell.Items.Add(abougPage);
            app = Substitute.For<Microsoft.Maui.Controls.Application>();
            app.MainPage = shell;

            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                builder.AddDebug()
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug));
            var logger = loggerFactory.CreateLogger<UserService>();
            loginViewModelLogger = loggerFactory.CreateLogger<LoginViewModel>();
            dataStore = new MockDataStore();
            userService = new UserService(dataStore, logger);
        }

        [Fact]
        public void LoginCommandTest() 
        {
            //TODO: Need to fix this
            //var loginservice = new LoginService(userService);
            //LoginViewModel vm = new(loginservice, loginViewModelLogger);
            //vm.Password = "passowrd";
            //vm.LoginCommand.Execute(null);
        }
    }
}
