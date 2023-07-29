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
            dataStore = new DataStore();
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
        public async Task RemoveUserTestAsync() 
        {
            UserService userService = new UserService(dataStore, logger);
            User user = new User();
            user.Username = "new user 1";

            await userService.AddUserAsync(user);
            Assert.True(user.IsUserExist);
            await userService.DeleteUserAsync(user);
            Assert.False(user.IsUserExist);
        }

        [Fact]
        public async Task LoginAsyncTest() 
        {
            UserService userService = new UserService(dataStore, logger);
            User user = new User();
            user.Username = "new user 1";
            user.Password = "12345";

            await userService.AddUserAsync(user);
            Assert.True(user.IsUserExist);
            await userService.LoginAsync(user);
            await userService.DeleteUserAsync(user);
            Assert.False(user.IsUserExist);
        }

        [Fact]
        public async Task LogoutTest() 
        {
            UserService userService = new UserService(dataStore, logger);
            User user = new User();
            user.Username = "new user 1";
            user.Password = "12345";

            await userService.AddUserAsync(user);
            Assert.True(user.IsUserExist);
            await userService.LoginAsync(user);
            userService.Logout();
            await userService.DeleteUserAsync(user);
            Assert.False(user.IsUserExist);
        }
    }
}
