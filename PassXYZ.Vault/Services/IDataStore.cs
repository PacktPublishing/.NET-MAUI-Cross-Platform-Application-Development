using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

using PassXYZLib;

namespace PassXYZ.Vault.Services;

public interface IDataStore<T>
{
    #region DS_misc
    T CurrentGroup { get; set; }
    string CurrentPath { get; }
    T RootGroup { get; }
    bool IsOpen { get; }
    string GetStoreName();
    DateTime GetStoreModifiedTime();
    Task<bool> MergeAsync(string path);
    // Methods to handle icons
    ObservableCollection<PxIcon> GetCustomIcons(string? searchText = null);
    Task<bool> DeleteCustomIconAsync(PxIcon icon);
    ImageSource GetBuiltInImage(PxIcon icon);
    #endregion

    #region DS_Item
    T? CreateNewItem(ItemSubType type);
    T? GetItem(string id, bool SearchRecursive = false);
    Task<T?> GetItemAsync(string id, bool SearchRecursive = false);
    Task AddItemAsync(T item);
    Task UpdateItemAsync(T item);
    Task<bool> DeleteItemAsync(string id);
    Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    Task<IEnumerable<T>> GetOtpEntryListAsync();
    Task<IEnumerable<T>> SearchEntriesAsync(string strSearch, T itemGroup);
    #endregion
    
}
