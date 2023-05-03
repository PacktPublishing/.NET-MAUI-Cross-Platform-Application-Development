using KPCLib;
using Microsoft.AspNetCore.Components;
using PassXYZ.BlazorUI;
using PassXYZLib;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PassXYZ.Vault.Tests;

public partial class ListViewTests : TestContext 
{
    readonly ObservableCollection<Item> items;
    NewItem testItem;
    string headerContent = "This is list view headker.";
    string footerContent = "This is list view footer.";

    public ListViewTests() 
    {
        items = new ObservableCollection<Item>();
        testItem = new ()
        {
            Name = "New item",
            Notes = "This is a new item."
        };
        items.Add(testItem);
    }

    [Fact]
    public void ListView_Init_WithoutParameters()
    {
        var cut = RenderComponent<ListView<Item>>();
        Assert.NotNull(cut.Find("div[class=list-group]"));
        Debug.WriteLine($"{cut.Markup}");
    }

    [Fact]
    public void Display_ListView_Items() 
    {
        var cut = Render(_listView);
        var item = cut.Find("a");
        Assert.Equal(testItem.Name, item.TextContent);
        Debug.WriteLine($"{cut.Markup}");
    }

    [Fact]
    public void Check_ListView_HeaderAndFooter() 
    {
        var cut = Render(_listView);
        Assert.Equal(headerContent, cut.Find("#list_view_header").TextContent);
        Assert.Equal(footerContent, cut.Find("article").TextContent);
    }
}
