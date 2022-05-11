using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

using PassXYZLib;

namespace PassXYZ.Vault.Services;

public interface IDataStore<T, U>
{
    #region User_management
    U CurrentUser { get; }
    bool IsBusyToLoadUsers { get; }
    ObservableCollection<U>? Users { get; }

    U GetUser(string username);
    Task AddUserAsync(U user);
    Task UpdateUserAsync(U user);
    Task DeleteUserAsync(U user);
    List<string> GetUsersList();
    Task<bool> SynchronizeUsersAsync();
    Task<bool> LoginAsync(U user);
    void Logout();
    #endregion

    #region Database_management
    // Database 
    T CurrentGroup { get; set; }
    string CurrentPath { get; }
    T RootGroup { get; }
    bool IsOpen { get; }
    string GetStoreName();
    DateTime GetStoreModifiedTime();
    string GetMasterPassword();
    string GetDeviceLockData();
    Task<bool> ChangeMasterPassword(string newPassword);
    bool CreateKeyFile(string data, string username);
    Task<bool> MergeAsync(string path);
    #endregion

    #region Item_management
    T? GetItem(string id, bool SearchRecursive = false);
    Task<T?> GetItemAsync(string id, bool SearchRecursive = false);
    Task AddItemAsync(T item);
    Task UpdateItemAsync(T item);
    Task<bool> DeleteItemAsync(string id);
    Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    Task<IEnumerable<T>> GetOtpEntryListAsync();
    Task<IEnumerable<T>> SearchEntriesAsync(string strSearch, T itemGroup);
    #endregion
    
    // Methods to handle icons
    ObservableCollection<PxIcon> GetCustomIcons(string? searchText = null);
    Task<bool> DeleteCustomIconAsync(PxIcon icon);
    ImageSource GetBuiltInImage(PxIcon icon);
}
