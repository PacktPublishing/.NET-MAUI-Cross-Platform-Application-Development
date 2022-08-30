using Microsoft.AspNetCore.Components;
using PassXYZLib;
using System.Diagnostics;

namespace PassXYZ.Vault.Tests;

public partial class ItemEditTests : TestContext
{
    public bool IsNewItem { get; set; } = false;
    public NewItem testItem { get; set; }
    string _dialogId = "editItem";
    string updated_key = "Updated item";
    string updated_value = "This item is updated.";

    public ItemEditTests()
    {
        testItem = new()
        {
            Name = "New item",
            Notes = "This is a new item."
        };
    }

    void OnSaveClicked(string key, string value)
    {
        testItem.Name = key;
        testItem.Notes = value;

        Debug.WriteLine($"ItemNew: OnSaveClicked(key={testItem.Name}, value={testItem.Notes}, type={testItem.ItemType})");
    }

    [Fact]
    public void Edit_New_Item()
    {
        // Arrange
        IsNewItem = true;
        var cut = Render(GetComponent());
        // Act
        cut.Find("#itemType").Change("Entry");
        cut.Find("input").Change(updated_key);
        cut.Find("textarea").Change(updated_value);
        cut.Find("button[type=submit]").Click();
        // Assert
        Assert.Equal(updated_key, testItem.Name);
        Assert.Equal(updated_value, testItem.Notes);
    }

    [Fact]
    public void Edit_Existing_Item() 
    {
        // Arrange
        IsNewItem = false;
        var cut = Render(GetComponent());
        var ex = Assert.Throws<ElementNotFoundException>(() => cut.Find("#itemType").Change("Entry"));
        Assert.Equal("No elements were found that matches the selector '#itemType'", ex.Message);
    }

}