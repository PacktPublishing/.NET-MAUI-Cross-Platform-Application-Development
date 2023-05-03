using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPCLib;
using PassXYZLib;
using Moq;
using PassXYZ.Vault.Pages;
using PassXYZ.Vault.Services;
using KeePassLib;

namespace PassXYZ.Vault.Tests;

[Collection("Serilog collection")]
public class ItemDetailTests : TestContext
{
    SerilogFixture serilogFixture;
    Mock<IDataStore<Item>> dataStore;

    public ItemDetailTests(SerilogFixture fixture) 
    {
        serilogFixture = fixture;
        dataStore = new Mock<IDataStore<Item>>();

        Services.AddSingleton<IDataStore<Item>>(dataStore.Object);
    }

    [Fact]
    public void Init_Empty_ItemDetail()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => RenderComponent<ItemDetail>());
        Assert.Equal("ItemDetail: SelectedItemId is null", ex.Message);
    }

    [Fact]
    public void Load_ItemDetail_WithWrongId()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
            RenderComponent<ItemDetail>(parameters => parameters.Add(p => p.SelectedItemId, "Wrong Id")));
        Assert.Equal("ItemDetail: entry cannot be found with SelectedItemId", ex.Message);
    }

    [Fact]
    public void Load_ItemDetail_WithGroup()
    {
        Item testGroup = new PwGroup(true, true)
        {
            Name = "Default Group",
            Notes = "This is a group in ItemDetailTests."
        };
        dataStore.Setup(x => x.GetItem(It.IsAny<string>())).Returns(testGroup);
        var ex = Assert.Throws<InvalidOperationException>(() =>
            RenderComponent<ItemDetail>(parameters => parameters.Add(p => p.SelectedItemId, testGroup.Id)));
        Assert.Equal("ItemDetail: SelectedItemId should not be group here.", ex.Message);
    }

    [Fact]
    public void Load_ItemDetail_WithEmptyFieldList() 
    {
        Item testEntry = new PwEntry(true, true) 
        { 
            Name = "Default Entry",
            Notes = "This is an entry with empty field list."
        };
        dataStore.Setup(x => x.GetItem(It.IsAny<string>())).Returns(testEntry);
        var cut = RenderComponent<ItemDetail>(parameters => parameters.Add(p => p.SelectedItemId, testEntry.Id));
        cut.Find("article").MarkupMatches($"<article><p>{testEntry.Notes}</p></article>");
        Debug.WriteLine($"{cut.Markup}");
    }
}
