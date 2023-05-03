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
    private readonly PasswordDb _db = default!;
    private PwGroup? _currentGroup = null;
    private bool _isBusy = false;
    public MockDataStore()
    {
        _db = PasswordDb.Instance;
        PwDatabaseEx.CreateTestDb(_db);
        _db.LastSelectedGroup = _db.RootGroup.Uuid;
        _currentGroup = _db.RootGroup;
        Debug.WriteLine("MockDataStore: db created.");
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

			return true;
		});
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

}
