using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

using PassXYZLib;

namespace PassXYZ.Vault.Services;

public interface IDataStore<T, U>
{
    Task AddItemAsync(T item);
    Task UpdateItemAsync(T item);
    Task<bool> DeleteItemAsync(string id);
    Task<T> GetItemFromCurrentGroupAsync(string id);
    T GetItemFromCurrentGroup(string id);
    T FindGroup(string id);
    Task<T?> FindEntryByIdAsync(string id);
    T? FindEntryById(string id);
    Task<T?> GetItemAsync(string id);
    Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    Task<IEnumerable<T>> SearchEntriesAsync(string strSearch, T itemGroup);
    Task<IEnumerable<T>> GetOtpEntryListAsync();
    T CurrentGroup { get; set; }
    string CurrentPath { get; }
    void SetCurrentToParent();
    Task SaveAsync();
    T RootGroup { get; }
    bool IsOpen { get; }
    Task<bool> LoginAsync(U user);
    void Logout();
    string GetStoreName();
    DateTime GetStoreModifiedTime();
    U CurrentUser { get; }
    Task SignUpAsync(U user);
    Task<bool> ChangeMasterPassword(string newPassword);
    string GetMasterPassword();
    string GetDeviceLockData();
    ObservableCollection<PxIcon> GetCustomIcons(string? searchText = null);
    Task<bool> DeleteCustomIconAsync(PxIcon icon);
    ImageSource GetBuiltInImage(PxIcon icon);
    bool CreateKeyFile(string data, string username);
    Task<bool> MergeAsync(string path);
    ObservableCollection<U>? Users { get;}
    List<string> GetUsersList();
    Task<bool> SynchronizeUsersAsync();
    bool IsBusyToLoadUsers { get; }
}
