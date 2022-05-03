using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

using PassXYZLib;

namespace PassXYZ.Vault.Services;

public interface IDataStore<T, U>
{
    // USER - Properties
    U CurrentUser { get; }
    bool IsBusyToLoadUsers { get; }
    ObservableCollection<U>? Users { get; }

    // USER - Methods
    List<string> GetUsersList();
    Task<bool> SynchronizeUsersAsync();
    Task<bool> LoginAsync(U user);
    void Logout();
    Task SignUpAsync(U user);

    // ITEM - Properties
    T CurrentGroup { get; set; }
    string CurrentPath { get; }
    T RootGroup { get; }
    bool IsOpen { get; }

    // ITEM - Methods to update data
    Task AddItemAsync(T item);
    Task UpdateItemAsync(T item);
    Task<bool> DeleteItemAsync(string id);
    void SetCurrentToParent();
    Task SaveAsync();
    Task<bool> ChangeMasterPassword(string newPassword);
    bool CreateKeyFile(string data, string username);
    Task<bool> MergeAsync(string path);

    // ITEM - Methods to read data
    Task<T> GetItemFromCurrentGroupAsync(string id);
    T GetItemFromCurrentGroup(string id);
    T FindGroup(string id);
    Task<T?> FindEntryByIdAsync(string id);
    T? FindEntryById(string id);
    Task<T?> GetItemAsync(string id);
    Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    Task<IEnumerable<T>> SearchEntriesAsync(string strSearch, T itemGroup);
    Task<IEnumerable<T>> GetOtpEntryListAsync();
    string GetStoreName();
    DateTime GetStoreModifiedTime();
    string GetMasterPassword();
    string GetDeviceLockData();

    // Methods to handle icons
    ObservableCollection<PxIcon> GetCustomIcons(string? searchText = null);
    Task<bool> DeleteCustomIconAsync(PxIcon icon);
    ImageSource GetBuiltInImage(PxIcon icon);
}
