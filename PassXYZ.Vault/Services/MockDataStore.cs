using KPCLib;
using PassXYZLib;

namespace PassXYZ.Vault.Services;

public class MockDataStore : IDataStore<Item>
{
    static string[] jsonData = {
        "pxtem://{'IsPxEntry':true,'Strings':{'000UserName':{'Value':'PassXYZ Tester','IsProtected':false},'001Password':{'Value':'','IsProtected':true},'002Email':{'Value':'','IsProtected':false},'003URL':{'Value':'https://www.facebook.com/','IsProtected':false},'004QQ':{'Value':'','IsProtected':false},'005WeChat':{'Value':'','IsProtected':false},'Notes':{'Value':'Social Media','IsProtected':false},'Title':{'Value':'Facebook','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':true,'Strings':{'000UserName':{'Value':'PassXYZ Tester','IsProtected':false},'001Password':{'Value':'12345','IsProtected':true},'002Email':{'Value':'user2@passxyz.com','IsProtected':false},'003URL':{'Value':'https://passxyz.github.io','IsProtected':false},'004QQ':{'Value':'1234567890','IsProtected':false},'005WeChat':{'Value':'passxyz','IsProtected':false},'Notes':{'Value':'Social media','IsProtected':false},'Title':{'Value':'Twitter','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':false,'Strings':{'Password':{'Value':'','IsProtected':true},'Mobile':{'Value':'','IsProtected':false},'Notes':{'Value':'Search Engine','IsProtected':false},'PIN':{'Value':'','IsProtected':true},'Title':{'Value':'Google','IsProtected':false},'URL':{'Value':'https://www.google.com/','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':false,'Strings':{'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'13678909876','IsProtected':false},'Notes':{'Value':'Programming','IsProtected':false},'PIN':{'Value':'123','IsProtected':true},'Title':{'Value':'GitHub','IsProtected':false},'URL':{'Value':'https://github.com','IsProtected':false},'UserName':{'Value':'test2','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':true,'Strings':{'000UserName':{'Value':'PassXYZ Tester','IsProtected':false},'002Email':{'Value':'tester@amazon.com','IsProtected':false},'003URL':{'Value':'https://www.amazon.com/','IsProtected':false},'004QQ':{'Value':'123456789','IsProtected':false},'005WeChat':{'Value':'passxyz','IsProtected':false},'Notes':{'Value':'Shopping','IsProtected':false},'Title':{'Value':'Amazon','IsProtected':false}}}"
    };

    readonly List<Item> items;

    public MockDataStore()
    {
        items = new List<Item>()
        {
            new PxEntry(jsonData[0]),
            new PxEntry(jsonData[1]),
            new PxEntry(jsonData[2]),
            new PxEntry(jsonData[3]),
            new PxEntry(jsonData[4])
        };
    }

    public async Task<bool> AddItemAsync(Item item)
    {
        items.Add(item);

        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateItemAsync(Item item)
    {
        var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
        items.Remove(oldItem);
        items.Add(item);

        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteItemAsync(string id)
    {
        var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
        items.Remove(oldItem);

        return await Task.FromResult(true);
    }

    public async Task<Item> GetItemAsync(string id)
    {
        return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
    }

    public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
    {
        return await Task.FromResult(items);
    }
}