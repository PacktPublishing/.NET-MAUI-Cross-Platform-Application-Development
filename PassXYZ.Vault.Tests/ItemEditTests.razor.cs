using Microsoft.AspNetCore.Components;
using PassXYZLib;
using System.Diagnostics;

namespace PassXYZ.Vault.Tests;

[Collection("Serilog collection")]
public partial class ItemEditTests : TestContext
{
    readonly SerilogFixture serilogFixture;
    bool isNewItem { get; set; } = false;
    NewItem testItem { get; set; }
    string _dialogId = "editItem";
    string updated_key = "Updated item";
    string updated_value = "This item is updated.";

    public ItemEditTests(SerilogFixture fixture)
    {
        testItem = new()
        {
            Name = "New item",
            Notes = "This is a new item."
        };
        serilogFixture = fixture;
    }

    void OnSaveClicked(string key, string value)
    {
        testItem.Name = key;
        testItem.Notes = value;

        serilogFixture.Logger.Debug($"ItemEditTests: OnSaveClicked(key={testItem.Name}, value={testItem.Notes}, type={testItem.ItemType})");
    }

    [Fact]
    public void Edit_New_Item()
    {
        // Arrange
        isNewItem = true;
        var cut = Render(_editorDialog);
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
        isNewItem = false;
        // Act
        var cut = Render(_editorDialog);
        var ex = Assert.Throws<ElementNotFoundException>(() => cut.Find("#itemType").Change("Entry"));
        Assert.Equal("No elements were found that matches the selector '#itemType'", ex.Message);
        cut.Find("textarea").Change(updated_value);
        cut.Find("button[type=submit]").Click();
        Assert.Equal(updated_value, testItem.Notes);
    }
}