using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassXYZ.Vault.Services;

public interface IUserService<T>
{
    T CurrentUser { get; set; }
    bool IsBusyToLoadUsers { get; }
    ObservableCollection<T>? Users { get; }
    T GetUser(string username);
    Task AddUserAsync(T user);
    Task UpdateUserAsync(T user);
    Task DeleteUserAsync(T user);
    List<string> GetUsersList();
    Task<bool> SynchronizeUsersAsync();
    Task<bool> LoginAsync(T user);
    void Logout();
    string GetMasterPassword();
    Task<bool> ChangeMasterPassword(string newPassword);
    string GetDeviceLockData();
    bool CreateKeyFile(string data, string username);
}
