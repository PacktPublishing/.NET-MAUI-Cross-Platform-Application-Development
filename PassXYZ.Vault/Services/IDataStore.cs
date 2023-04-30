using PassXYZLib;

namespace PassXYZ.Vault.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        T? GetItem(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
        string SetCurrentGroup(T? group = default);
        Task<bool> ConnectAsync(User user);
        T? CreateNewItem(ItemSubType type);
    }
}
