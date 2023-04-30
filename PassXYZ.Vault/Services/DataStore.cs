using System.Diagnostics;
using System.Collections.ObjectModel;

using SkiaSharp;

using KeePassLib;
using KPCLib;
using KeePassLib.Collections;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using PassXYZLib;

using PassXYZ.Vault.Properties;
using Newtonsoft.Json.Linq;

namespace PassXYZ.Vault.Services;

public class EmbeddedDatabase
{
    public string Path;
    public string Key;
    public string ResourcePath;

    public EmbeddedDatabase(string path, string key, string rpath)
    {
        Path = path;
        Key = key;
        ResourcePath = rpath;
    }
}

public static class EmbeddedIcons
{
    public static EmbeddedDatabase iconZipFile = new EmbeddedDatabase(Path.Combine(PxDataFile.TmpFilePath, "icons.zip"),
        "icons", "PassXYZ.Vault.data.icons.zip");

    public static EmbeddedDatabase[] IconFiles = new EmbeddedDatabase[]
    {
            new EmbeddedDatabase(Path.Combine(PxDataFile.IconFilePath, "ic_passxyz_cloud.png"), "ic_passxyz_cloud", "PassXYZ.Vault.data.ic_passxyz_cloud.png"),
            new EmbeddedDatabase(Path.Combine(PxDataFile.IconFilePath, "ic_passxyz_local.png"), "ic_passxyz_local", "PassXYZ.Vault.data.ic_passxyz_local.png"),
            new EmbeddedDatabase(Path.Combine(PxDataFile.IconFilePath, "ic_passxyz_disabled.png"), "ic_passxyz_disabled", "PassXYZ.Vault.data.ic_passxyz_disabled.png"),
            new EmbeddedDatabase(Path.Combine(PxDataFile.IconFilePath, "ic_passxyz_sync.png"), "ic_passxyz_sync", "PassXYZ.Vault.data.ic_passxyz_sync.png"),
            new EmbeddedDatabase(Path.Combine(PxDataFile.IconFilePath, "ic_passxyz_synced.png"), "ic_passxyz_synced", "PassXYZ.Vault.data.ic_passxyz_synced.png")
    };
}

public static class TEST_DB
{
    public static EmbeddedDatabase[] DataFiles = new EmbeddedDatabase[]
    {
            new EmbeddedDatabase(Path.Combine(PxDataFile.DataFilePath, "pass_d_E8f4pEk.xyz"), "12345", "PassXYZ.Vault.data.pass_d_E8f4pEk.xyz"),
            new EmbeddedDatabase(Path.Combine(PxDataFile.DataFilePath, "pass_e_JyHzpRxcopt.xyz"), "123123", "PassXYZ.Vault.data.pass_e_JyHzpRxcopt.xyz"),
            new EmbeddedDatabase(Path.Combine(PxDataFile.KeyFilePath, "pass_k_JyHzpRxcopt.k4xyz"), "", "PassXYZ.Vault.data.pass_k_JyHzpRxcopt.k4xyz")
    };
}

public class DataStore : IDataStore<Item>
{
    private readonly PasswordDb _db = default!;
    private PwGroup? _currentGroup = null;
    private bool _isBusy = false;
    public DataStore()
    {
        _db = PasswordDb.Instance;
    }

    public List<Item> Items => _currentGroup!.GetItems();

    /// <summary>
    /// Set the current group.
    /// If the group is null, set to root group.
    /// </summary>
    /// <param name="group">an instance of <c>PwGroup</c></param>
    /// <returns>Returns the current group name</returns>
    public string SetCurrentGroup(Item? item = default)
    {
        if(item == default) 
        { 
            _currentGroup = _db.RootGroup;
            return _db.RootGroup.Name;
        }

        if (item is PwGroup group)
        {
            _db.LastSelectedGroup = group.Uuid;
            if (_db.RootGroup.Uuid == _db.LastSelectedGroup || _db.LastSelectedGroup.Equals(PwUuid.Zero))
            {
                _db.LastSelectedGroup = _db.RootGroup.Uuid;
                _currentGroup = _db.RootGroup;
            }
            else
                _currentGroup = group;
            return _currentGroup.Name;
        }
        else 
        {
            throw new ArgumentException("Item must be a group", nameof(item));
        }
    }

    private async Task SaveAsync()
    {
        Debug.WriteLine($"DataStore: SaveAsync _isBusy={_isBusy}");
        await Task.FromResult(true);
    }

    public async Task<bool> AddItemAsync(Item item)
    {
        if (item == null || _currentGroup == null) { throw new ArgumentNullException(nameof(item)); }
        //Items.Add(item);
        if (item.IsGroup)
        {
            _currentGroup.AddGroup(item as PwGroup, true);
        }
        else
        {
            _currentGroup.AddEntry(item as PwEntry, true);
        }
        await SaveAsync();
        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateItemAsync(Item item)
    {
        if(item == null) { throw new ArgumentNullException(nameof(item)); }
        var oldItem = Items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
        if (oldItem != null) 
        {
            item.LastModificationTime = DateTime.UtcNow;
            await SaveAsync();
            return await Task.FromResult(true);
        }
        else
        {
            return await Task.FromResult(false);
        }
    }

    public async Task<bool> DeleteItemAsync(string id)
    {
        if (id == null) { throw new ArgumentNullException(nameof(id)); }

        Item? oldItem = Items.Where((Item arg) => arg.Id == id).FirstOrDefault();

        if (oldItem != null)
        {
            Items.Remove(oldItem);
            await SaveAsync();
            return await Task.FromResult(true);
        }
        else
        {
            return await Task.FromResult(false);
        }
    }

    public Item? GetItem(string id)
    {
        if (id == null) { throw new ArgumentNullException(nameof(id)); }

        var item = Items.FirstOrDefault(s => s.Id == id);
        if (item != null) { return item; }

        item = _db.FindEntryById(id);
        if (item != null) { return item; }
        else
        {
            return _db.RootGroup.FindGroup(id, true);
        }
    }

    public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
    {
        if (_currentGroup == null)
        {
            Debug.WriteLine($"MockDataStore: CurrentGroup is null!");
            return Enumerable.Empty<Item>();
        }
        return await Task.FromResult(Items);
    }

    public async Task<bool> ConnectAsync(User user)
    {
        return await Task.Run(() =>
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password)) 
            {
                throw new ArgumentNullException(nameof(user), "Username or password cannot be null");
            }

            _db.Open(user);
            if (_db.IsOpen)
            {
                _db.CurrentGroup = _db.RootGroup;
            }
            return _db.IsOpen;
        });
    }
}
