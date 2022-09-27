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

namespace PassXYZ.Vault.Services;

public static class PwDatabaseEx
{
    static string[] jsonData = {
        "pxtem://{'IsPxEntry':true,'Strings':{'000UserName':{'Value':'PassXYZ Tester','IsProtected':false},'001Password':{'Value':'','IsProtected':true},'002Email':{'Value':'','IsProtected':false},'003URL':{'Value':'https://www.facebook.com/','IsProtected':false},'004QQ':{'Value':'','IsProtected':false},'005WeChat':{'Value':'','IsProtected':false},'Notes':{'Value':'Social Media','IsProtected':false},'Title':{'Value':'Facebook','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':true,'Strings':{'000UserName':{'Value':'PassXYZ Tester','IsProtected':false},'001Password':{'Value':'12345','IsProtected':true},'002Email':{'Value':'user2@passxyz.com','IsProtected':false},'003URL':{'Value':'https://passxyz.github.io','IsProtected':false},'004QQ':{'Value':'1234567890','IsProtected':false},'005WeChat':{'Value':'passxyz','IsProtected':false},'Notes':{'Value':'Social media','IsProtected':false},'Title':{'Value':'Twitter','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':false,'Strings':{'Password':{'Value':'','IsProtected':true},'Mobile':{'Value':'','IsProtected':false},'Notes':{'Value':'Search Engine','IsProtected':false},'PIN':{'Value':'','IsProtected':true},'Title':{'Value':'Google','IsProtected':false},'URL':{'Value':'https://www.google.com/','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':false,'Strings':{'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'13678909876','IsProtected':false},'Notes':{'Value':'Programming','IsProtected':false},'PIN':{'Value':'123','IsProtected':true},'Title':{'Value':'GitHub','IsProtected':false},'URL':{'Value':'https://github.com','IsProtected':false},'UserName':{'Value':'test2','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':true,'Strings':{'000UserName':{'Value':'PassXYZ Tester','IsProtected':false},'002Email':{'Value':'tester@amazon.com','IsProtected':false},'003URL':{'Value':'https://www.amazon.com/','IsProtected':false},'004QQ':{'Value':'123456789','IsProtected':false},'005WeChat':{'Value':'passxyz','IsProtected':false},'Notes':{'Value':'Shopping','IsProtected':false},'Title':{'Value':'Amazon','IsProtected':false}}}"
    };

    static string[] jsonBanks = {
        "pxtem://{'IsPxEntry':false,'Strings':{'Title':{'Value':'Chase','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false},'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'1234567890','IsProtected':false},'Notes':{'Value':'Bank','IsProtected':false},'PIN':{'Value':'1234','IsProtected':true},'URL':{'Value':'https://www.chase.com','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':false,'Strings':{'Title':{'Value':'Bank of America ','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false},'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'1234567890','IsProtected':false},'Notes':{'Value':'Bank','IsProtected':false},'PIN':{'Value':'1234','IsProtected':true},'URL':{'Value':'https://www.bankofamerica.com','IsProtected':false}}}"
    };

    static string[] jsonBrokers = {
        "pxtem://{'IsPxEntry':false,'Strings':{'Title':{'Value':'Fidelity','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false},'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'1234567890','IsProtected':false},'Notes':{'Value':'Bank','IsProtected':false},'PIN':{'Value':'1234','IsProtected':true},'URL':{'Value':'https://www.fidelity.com','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':false,'Strings':{'Title':{'Value':'Ameritrade','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false},'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'1234567890','IsProtected':false},'Notes':{'Value':'Bank','IsProtected':false},'PIN':{'Value':'1234','IsProtected':true},'URL':{'Value':'https://www.tdameritrade.com/','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':false,'Strings':{'Title':{'Value':'E*TRADE','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false},'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'1234567890','IsProtected':false},'Notes':{'Value':'Bank','IsProtected':false},'PIN':{'Value':'1234','IsProtected':true},'URL':{'Value':'https://us.etrade.com/','IsProtected':false}}}"
    };

    static string[] jsonInsurance = {
        "pxtem://{'IsPxEntry':false,'Strings':{'Title':{'Value':'AIG','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false},'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'1234567890','IsProtected':false},'Notes':{'Value':'Bank','IsProtected':false},'PIN':{'Value':'1234','IsProtected':true},'URL':{'Value':'https://www.aig.com/','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':false,'Strings':{'Title':{'Value':'AIA','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false},'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'1234567890','IsProtected':false},'Notes':{'Value':'Bank','IsProtected':false},'PIN':{'Value':'1234','IsProtected':true},'URL':{'Value':'https://www.aia.com/','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':false,'Strings':{'Title':{'Value':'Prudential','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false},'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'1234567890','IsProtected':false},'Notes':{'Value':'Bank','IsProtected':false},'PIN':{'Value':'1234','IsProtected':true},'URL':{'Value':'https://www.prudential.com/','IsProtected':false}}}"
    };

    public static void CreateTestDb(PwDatabase pwDb)
    {
        pwDb.RootGroup = new PwGroup(true, true)
        {
            Name = "Password Data",
            Notes = "Store your password data in this database.",
        };

        PwGroup gMoney = new PwGroup(true, true)
        {
            Name = "Money",
            Notes = "financial institutions",
        };

        // Add some entries to gMoney
        #region Deposits

        PwGroup gDeposits = new PwGroup(true, true)
        {
            Name = "Deposits",
            Notes = "My bank accounts",
        };
        gDeposits.AddEntry(new PxEntry(jsonBanks[0]), true);
        gDeposits.AddEntry(new PxEntry(jsonBanks[1]), true);
        gMoney.AddGroup(gDeposits, true);

        pwDb.RootGroup.AddGroup(gMoney, true);
        #endregion

        #region Investment
        PwGroup gInvestment = new PwGroup(true, true)
        {
            Name = "Investment",
            Notes = "My investment"
        };
        PwGroup gStocks = new PwGroup(true, true)
        {
            Name = "Stocks",
            Notes = "My Stocks"
        };
        gStocks.AddEntry(new PxEntry(jsonBrokers[0]), true);
        gStocks.AddEntry(new PxEntry(jsonBrokers[1]), true);
        gStocks.AddEntry(new PxEntry(jsonBrokers[2]), true);
        gInvestment.AddGroup(gStocks, true);

        PwGroup gInsurance = new PwGroup(true, true)
        {
            Name = "Insurance policies",
            Notes = "My insurance policies"
        };
        gInsurance.AddEntry(new PxEntry(jsonInsurance[0]), true);
        gInsurance.AddEntry(new PxEntry(jsonInsurance[1]), true);
        gInsurance.AddEntry(new PxEntry(jsonInsurance[2]), true);
        gInvestment.AddGroup(gInsurance, true);

        pwDb.RootGroup.AddGroup(gInvestment, true);
        #endregion


        pwDb.RootGroup.AddEntry(new PxEntry(jsonData[0]), true);
        pwDb.RootGroup.AddEntry(new PxEntry(jsonData[1]), true);
        pwDb.RootGroup.AddEntry(new PxEntry(jsonData[2]), true);
        pwDb.RootGroup.AddEntry(new PxEntry(jsonData[3]), true);
        pwDb.RootGroup.AddEntry(new PxEntry(jsonData[4]), true);
    }
}

public class MockDataStore : IDataStore<Item>
{
    public MockDataStore()
    {
        db = PasswordDb.Instance;
        PwDatabaseEx.CreateTestDb(db);
        db.LastSelectedGroup = db.RootGroup.Uuid;
        currentGroup = db.RootGroup;
        Debug.WriteLine("MockDataStore: db created.");
    }

    private readonly PasswordDb db = default!;
    private bool _isBusy = false;

    public bool IsOpen => db != null && db.IsOpen;

    public Item RootGroup
    {
        get => db.RootGroup;
    }

    public string CurrentPath => db.CurrentPath;

    public List<Item> Items => currentGroup!.GetItems();

    private PwGroup? currentGroup = null;
    public Item CurrentGroup
    {
        get
        {
            if (db.RootGroup != null)
            {
                if (db.RootGroup.Uuid == db.LastSelectedGroup || db.LastSelectedGroup.Equals(PwUuid.Zero))
                {
                    db.LastSelectedGroup = db.RootGroup.Uuid;
                    currentGroup = db.RootGroup;
                }

                if (currentGroup == null)
                {
                    if (!db.LastSelectedGroup.Equals(PwUuid.Zero)) { currentGroup = db.RootGroup.FindGroup(db.LastSelectedGroup, true); }
                }
            }
            return currentGroup;
        }

        set
        {
            if (value == null) { Debug.Assert(false); throw new ArgumentNullException("value"); }
            if (value is PwGroup group)
            {
                db.LastSelectedGroup = group.Uuid;
                if (db.RootGroup.Uuid == db.LastSelectedGroup || db.LastSelectedGroup.Equals(PwUuid.Zero))
                {
                    db.LastSelectedGroup = db.RootGroup.Uuid;
                    currentGroup = db.RootGroup;
                }
                else
                    currentGroup = group;
            }
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
        Debug.WriteLine($"DataStore: SaveAsync _isBusy={_isBusy}");
        _ = await GetItemsAsync();
    }

    public async Task AddItemAsync(Item item)
    {
        Items.Add(item);
        if (item.IsGroup)
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
        item.LastModificationTime = DateTime.UtcNow;
        await SaveAsync();
        Debug.WriteLine($"DataStore: UpdateItemAsync({item.Name}), saved");
    }

    public async Task<bool> DeleteItemAsync(string id)
    {
        Item? oldItem = Items.Where((Item arg) => arg.Id == id).FirstOrDefault();

        if (oldItem != null) 
        {
            _ = Items.Remove(oldItem);
        }

        return await Task.FromResult(true);
    }

    public async Task<Item> GetItemFromCurrentGroupAsync(string id)
    {
        return await Task.FromResult(Items.FirstOrDefault(s => s.Id == id));
    }

    public Item GetItemFromCurrentGroup(string id)
    {
        return Items.FirstOrDefault(s => s.Id == id);
    }

    public Item FindGroup(string id)
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

        if(SearchRecursive) 
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
        if (currentGroup == null) 
        {
            Debug.WriteLine($"MockDataStore: CurrentGroup is null!");
            return Enumerable.Empty<Item>(); 
        }
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
    /// Delete a custom icon by uuid.
    /// </summary>
    /// <param name="icon">the custom icon</param>
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
    /// <param name="uuid">UUID of custom icon</param>
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
