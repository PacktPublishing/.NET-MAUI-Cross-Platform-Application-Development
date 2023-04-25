using Microsoft.Extensions.Logging;
using KPCLib;
using User = PassXYZLib.User;
using PassXYZ.Vault.Services;

namespace PassXYZ.Vault.Tests.Services
{
    public class UserServiceTests
    {
        ILogger<UserService> logger;
        readonly IDataStore<Item> dataStore;

        public UserServiceTests()
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                builder.AddDebug()
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug));
            logger = loggerFactory.CreateLogger<UserService>();
            dataStore = new MockDataStore();
        }

        [Fact]
        public void GetUserTest() 
        {
            UserService userService = new UserService(dataStore, logger);
            var user = userService.GetUser("test");
        }

        [Fact]
        public void GetUsersListTest() 
        {
            UserService userService = new UserService(dataStore, logger);
            var list = userService.GetUsersList();
        }

        [Fact]
        public void AddUserTest() 
        {
            UserService userService = new UserService(dataStore, logger);
            User user = new User();
            user.Username = "new user 1";

            _ = userService.AddUserAsync(user);
        }

        [Fact]
        public void RemoveUserTest() 
        {
            UserService userService = new UserService(dataStore, logger);
            User user = new User();
            user.Username = "new user 1";

            _ = userService.AddUserAsync(user);
            _ = userService.DeleteUserAsync(user);
        }

        [Fact]
        public void UpdateUserTest() 
        {
            UserService userService = new UserService(dataStore, logger);
            User user = new User();
            user.Username = "new user 1";

            _ = userService.AddUserAsync(user);
            _ = userService.UpdateUserAsync(user);
        }

        [Fact]
        public void LoginAsyncTest() 
        {
            UserService userService = new UserService(dataStore, logger);
            User user = new User();
            user.Username = "new user 1";

            _ = userService.LoginAsync(user);
        }

        [Fact]
        public void LogoutTest() 
        {
            UserService userService = new UserService(dataStore, logger);
            userService.Logout();
        }
    }
}
