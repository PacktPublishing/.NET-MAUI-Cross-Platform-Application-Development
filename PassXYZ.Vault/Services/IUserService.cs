using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassXYZ.Vault.Services;

public interface IUserService<T>
{
    T GetUser(string username);
    Task AddUserAsync(T user);
    Task UpdateUserAsync(T user);
    Task DeleteUserAsync(T user);
    List<string> GetUsersList();
    Task<bool> LoginAsync(T user);
    void Logout();

}
