using KPCLib;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using User = PassXYZLib.User;

namespace PassXYZ.Vault.Services;

public class UserService : IUserService<User>
{
    readonly IDataStore<Item> dataStore;
    ILogger<UserService> logger;
    private User? _user = default;

    public UserService(IDataStore<Item> dataStore, ILogger<UserService> logger) 
    {
        this.dataStore = dataStore;
        this.logger = logger;
    }

    public User GetUser(string username)
    {
        User user = new User();
        user.Username = username;
        logger.LogDebug($"Path={user.Path}");
        return user;
    }

    public async Task UpdateUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public List<string> GetUsersList()
    {
        return User.GetUsersList();
    }

    public async Task AddUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> LoginAsync(User user)
    {
        if (user == null) { throw new ArgumentNullException("user"); }
        _user = user;

        return true;
    }

    public void Logout() 
    {
        throw new NotImplementedException();
    }

}
