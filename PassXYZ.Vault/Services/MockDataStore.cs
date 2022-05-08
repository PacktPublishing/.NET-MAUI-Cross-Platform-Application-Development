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

public class MockDataStore : IDataStore<Item, User>
{
    public ObservableCollection<User> Users { get; private set; }
    private static readonly object _sync = new object();
    private static bool _isBusyToLoadUsers = false;
    public bool IsBusyToLoadUsers
    {
        get => _isBusyToLoadUsers;
        private set
        {
            lock (_sync)
            {
                _isBusyToLoadUsers = value;
            }
        }
    }

    public MockDataStore()
    {
        db = PasswordDb.Instance;
        PwDatabaseEx.CreateTestDb(db);
        db.LastSelectedGroup = db.RootGroup.Uuid;
        currentGroup = db.RootGroup;
        Users = new ObservableCollection<User>();
        SynchronizeUsersAsync();
        Debug.WriteLine("MockDataStore: db created.");
    }

    private readonly PasswordDb db = default!;
    private User _user;
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

    public User CurrentUser
    {
        get => _user;
    }

    public List<string> GetUsersList()
    {
        List<string> userList = new List<string>();

        if(Users != null)
        {
            foreach (User user in Users)
            {
                userList.Add(user.Username);
            }
        }
        return userList;
    }

    public async Task<bool> SynchronizeUsersAsync()
    {
        IEnumerable<PxUser> pxUsers = null;

#if PASSXYZ_CLOUD_SERVICE
            if (PxCloudConfig.IsConfigured && PxCloudConfig.IsEnabled)
            {
                if (PassXYZ.Vault.App.IsSshOperationTimeout)
                {
                    // If the last connection is timeout, we load local users first.
                    pxUsers = await PxUser.LoadLocalUsersAsync();
                }
                else 
                {
                    ICloudServices<PxUser> sftp = PxCloudConfig.GetCloudServices();
                    pxUsers = await sftp.SynchronizeUsersAsync();
                }
            }
            else
#endif // PASSXYZ_CLOUD_SERVICE
        {
            pxUsers = await PxUser.LoadLocalUsersAsync(IsBusyToLoadUsers);
        }

        if (pxUsers != null && Users != null)
        {
            IsBusyToLoadUsers = true;
            Users.Clear();
            foreach (PxUser pxUser in pxUsers)
            {
                Users.Add(pxUser);
            }
            IsBusyToLoadUsers = false;
            if (Users.Count > 0)
            {
                // We need to check whether the current user at App level exist
                if (!Users.Contains(App.CurrentUser) && !string.IsNullOrEmpty(App.CurrentUser.Username))
                {
                    Debug.WriteLine($"LoginViewModel: Username={App.CurrentUser.Username} doesn't existed.");
                    App.CurrentUser.Username = string.Empty;
                }
            }

            return true;
        }
        else
        {
            Debug.WriteLine("LoginViewModel: SynchronizeUsersAsync failed");
            return false;
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

    public async Task<Item?> GetItemAsync(string id)
    {
        return await Task.FromResult(Items.FirstOrDefault(s => s.Id == id));
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

    public async Task<bool> LoginAsync(User user)
    {
        if (user == null) { Debug.Assert(false); throw new ArgumentNullException("user"); }
        _user = user;

        return true;
#if MockDataStore
        return await Task.Run(() =>
        {
            if (string.IsNullOrEmpty(user.Password)) { return false; }

            db.Open(user);
            if (db.IsOpen)
            {
                db.CurrentGroup = db.RootGroup;
            }
            return db.IsOpen;
        });
#endif
    }

    public async Task SignUpAsync(PassXYZLib.User user)
    {
        if (user == null) { Debug.Assert(false); throw new ArgumentNullException("user"); }

        var logger = new KPCLibLogger();
        await Task.Run(() => {
            db.New(user);
            // Create a PassXYZ Usage note entry
            PwEntry pe = new PwEntry(true, true);
            pe.Strings.Set(PxDefs.TitleField, new ProtectedString(false, Properties.Resources.entry_id_passxyz_usage));
            pe.Strings.Set(PxDefs.NotesField, new ProtectedString(false, Properties.Resources.about_passxyz_usage));
            //pe.CustomData.Set(Item.TemplateType, ItemSubType.Notes.ToString());
            //pe.CustomData.Set(Item.PxIconName, "ic_entry_passxyz.png");
            pe.SetType(ItemSubType.Notes);
            db.RootGroup.AddEntry(pe, true);

            try
            {
                logger.StartLogging("Saving database ...", true);
                db.DescriptionChanged = DateTime.UtcNow;
                db.Save(logger);
                logger.EndLogging();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to save database." + e.Message);
            }
        });

    }

    public void Logout()
    {
        if (db.IsOpen) { db.Close(); }
        Debug.WriteLine("DataStore.Logout(done)");
    }

    public string GetStoreName()
    {
        return db.Name;
    }

    public DateTime GetStoreModifiedTime()
    {
        return db.DescriptionChanged;
    }

    public async Task<bool> ChangeMasterPassword(string newPassword)
    {
        bool result = db.ChangeMasterPassword(newPassword, _user);
        if (result)
        {
            db.MasterKeyChanged = DateTime.UtcNow;
            // Save the database to take effect
            await SaveAsync();
        }
        return result;
    }

    public string GetMasterPassword()
    {
        var userKey = db.MasterKey.GetUserKey(typeof(KcpPassword)) as KcpPassword;
        return userKey.Password.ReadString();
    }

    public string GetDeviceLockData()
    {
        return db.GetDeviceLockData(_user);
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

    /// <summary>
    /// Recreate a key file from a PxKeyData
    /// </summary>
    /// <param name="data">PxKeyData source</param>
    /// <param name="username">username inside PxKeyData source</param>
    /// <returns>true - created key file, false - failed to create key file.</returns>
    public bool CreateKeyFile(string data, string username)
    {
        return db.CreateKeyFile(data, username);
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
