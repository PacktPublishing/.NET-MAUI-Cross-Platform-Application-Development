using System.Diagnostics;
using System.Collections.ObjectModel;

using SkiaSharp;

using KPCLib;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using PassXYZLib;

using PassXYZ.Vault.Properties;

namespace PassXYZ.Vault.Services
{
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
        // private List<Item> items;
        private readonly PasswordDb db;

        public DataStore()
        {
            db = PasswordDb.Instance;
        }

        public bool IsOpen => db != null && db.IsOpen;

        public Item RootGroup
        {
            get => db.RootGroup;
        }

        public string CurrentPath => db.CurrentPath;

        public List<Item> Items => db.CurrentGroup!.GetItems();

        public Item CurrentGroup
        {
            get => db.CurrentGroup;
            set
            {
                db.CurrentGroup = (PwGroup)value;
            }
        }

        public void SetCurrentToParent()
        {
            if (!CurrentGroup.GetUuid().Equals(RootGroup.GetUuid()))
            {
                CurrentGroup = db.CurrentGroup.ParentGroup;
            }
        }

        public async Task SaveAsync()
        {
            await db.SaveAsync();
#if PASSXYZ_CLOUD_SERVICE
            _user.CurrentFileStatus.IsModified = true;
#endif // PASSXYZ_CLOUD_SERVICE
            _ = await GetItemsAsync();
        }

        public async Task AddItemAsync(Item item)
        {
            Items.Add(item);
            if(item.IsGroup)
            {
                db.CurrentGroup.AddGroup(item as PwGroup, true);
            }
            else
            {
                db.CurrentGroup.AddEntry(item as PwEntry, true);
            }
            await SaveAsync();
            Debug.WriteLine($"DataStore: AddItemAsync({item.Name}), saved");
        }

        public async Task UpdateItemAsync(Item item)
        {
            //var oldItem = items.Where((Item arg) => arg.Uuid == item.Uuid).FirstOrDefault();
            //items.Remove(oldItem);
            //items.Add(item);
            item.LastModificationTime = DateTime.UtcNow;
            await SaveAsync();
            Debug.WriteLine($"DataStore: UpdateItemAsync({item.Name}), saved");
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            Item oldItem = Items.FirstOrDefault((Item arg) => arg.Id == id);
            if ((oldItem != null) && Items.Remove(oldItem))
            {
                if (oldItem.IsGroup)
                {
                    db.DeleteGroup(oldItem as PwGroup);
                }
                else
                {
                    db.DeleteEntry(oldItem as PwEntry);
                }
                await SaveAsync();
                Debug.WriteLine($"DataStore: DeleteItemAsync({oldItem.Name}), saved");
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<Item> GetItemFromCurrentGroupAsync(string id)
        {
            return await Task.FromResult(Items.FirstOrDefault(s => s.Id == id));
        }

        public Item GetItemFromCurrentGroup(string id)
        {
            return Items.FirstOrDefault(s => s.Id == id);
        }

        public PwGroup FindGroup(string id)
        {
            return db.RootGroup.FindGroup(id, true);
        }

        public async Task<Item?> FindEntryByIdAsync(string id)
        {
            return await Task.Run(() => { return db.FindEntryById(id); });
        }

        public Item? FindEntryById(string id)
        {
            return db.FindEntryById(id);
        }

        /// <summary>
        /// This is a factory method to create a new item.
        /// </summary>
        /// <param name="type">type of item</param>
        /// <returns>an instance of PwEntry or PwGroup</returns>
        public Item? CreateNewItem(ItemSubType type)
        {
            Item? newItem = default;

            if (type == ItemSubType.Group)
            {
                newItem = new PwGroup(true, true);
            }
            else if (type != ItemSubType.None)
            {
                PwEntry entry = new PwEntry(true, true);
                entry.SetType(type);

                // Init standard field
                if (type == ItemSubType.Entry)
                {
                    entry.Strings.Set(PwDefs.UserNameField, new ProtectedString(false, ""));
                    entry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, ""));
                    entry.Strings.Set(PwDefs.UrlField, new ProtectedString(false, ""));
                }
                else if (type == ItemSubType.PxEntry)
                {
                    uint idx = 0;
                    entry.Strings.Set(PxDefs.EncodeKey(Properties.Resources.field_id_username, idx++), new ProtectedString(false, ""));
                    entry.Strings.Set(PxDefs.EncodeKey(Properties.Resources.field_id_password, idx++), new ProtectedString(true, ""));
                    entry.Strings.Set(PxDefs.EncodeKey(Properties.Resources.field_id_url, idx++), new ProtectedString(false, ""));
                    entry.Strings.Set(PxDefs.EncodeKey(Properties.Resources.field_id_email, idx++), new ProtectedString(false, ""));
                    entry.Strings.Set(PxDefs.EncodeKey(Properties.Resources.field_id_mobile, idx++), new ProtectedString(false, ""));
                }

                newItem = entry;
            }
            return newItem;
        }

        public Item? GetItem(string id, bool SearchRecursive = false)
        {
            var item = Items.FirstOrDefault(s => s.Id == id);
            if (item != null) { return item; }

            if (SearchRecursive)
            {
                item = db.FindEntryById(id);
                if (item != null) { return item; }
                else
                {
                    return FindGroup(id);
                }
            }
            return item;
        }

        public async Task<Item?> GetItemAsync(string id, bool SearchRecursive = false)
        {
            return await Task.FromResult(GetItem(id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(Items);
        }

        /// <summary>
        /// Search entries with a keyword
        /// </summary>
        /// <param name="strSearch">keyword to be searched</param>
        /// <param name="itemGroup">If it is not null, this group is searched</param>
        /// <returns>a list of entries</returns>
        public async Task<IEnumerable<Item>> SearchEntriesAsync(string strSearch, Item itemGroup = null)
        {
            return await Task.Run(() => { return db.SearchEntries(strSearch, itemGroup); });
        }

        public async Task<IEnumerable<Item>> GetOtpEntryListAsync()
        {
            return await Task.Run(() => { return db.GetOtpEntryList(); });
        }

        public string GetStoreName()
        {
            return db.Name;
        }

        public DateTime GetStoreModifiedTime()
        {
            return db.DescriptionChanged;
        }

        /// <summary>
        /// Get the list of custom icons.
        /// </summary>
        /// <returns>list of custom icons</returns>
        public ObservableCollection<PxIcon> GetCustomIcons(string? searchText = null)
        {
            ObservableCollection<PxIcon> icons = new();
            foreach (PwCustomIcon pwci in db.CustomIcons)
            {
                if ((searchText == null) || pwci.Name.Contains(searchText))
                {
                    PxIcon icon = new PxIcon
                    {
                        IconType = PxIconType.PxEmbeddedIcon,
                        Uuid = pwci.Uuid,
                        Name = pwci.Name,
                    };
                    icon.ImgSource = GetBuiltInImage(icon);
                    icons.Add(icon);
                }
            }

            return icons;
        }

        /// <summary>
        /// Get the list of custom icons.
        /// </summary>
        /// <returns>list of custom icons</returns>
        public List<PwCustomIcon> GetCustomIcons()
        {
            return db.CustomIcons;
        }

        /// <summary>
        /// Delete a custom icon by uuid.
        /// </summary>
        /// <param name="icon">custom icon</param>
        /// <returns>success or failure</returns>
        public async Task<bool> DeleteCustomIconAsync(PxIcon icon)
        {
            List<PwUuid> vUuidsToDelete = new List<PwUuid>();

            if (icon.Uuid == null) { return false; }
            vUuidsToDelete.Add(icon.Uuid);
            bool result = db.DeleteCustomIcons(vUuidsToDelete);
            if (result)
            {
                // Save the database to take effect
                await SaveAsync();
            }
            return result;
        }

        /// <summary>
        /// Get the image source from the custom icon in the database
        /// </summary>
        /// <param name="icon">Custom icon</param>
        /// <returns>ImageSource or null</returns>
        public ImageSource GetBuiltInImage(PxIcon icon)
        {
            PwCustomIcon customIcon = db.GetPwCustomIcon(icon.Uuid);
            if (customIcon != null)
            {
                byte[] pb = customIcon.ImageDataPng;
                SKBitmap bitmap = PxItem.LoadImage(pb);
                return PxItem.GetImageSource(bitmap);
            }
            return null;
        }

        public async Task<bool> MergeAsync(string path)
        {
            return await Task.Run(() =>
            {
                bool result = false;
                if (db.IsOpen)
                {
                    result = db.Merge(path, PwMergeMethod.KeepExisting);
                }
                return result;
            });
        }
    }
}