using KPCLib;
using Microsoft.AspNetCore.Components;
using PassXYZLib;
using System.Diagnostics;

namespace PassXYZ.Vault.Tests;

public partial class FieldEditTests : TestContext 
{
    public bool IsNewField { get; set; } = false;
    public Field TestField { get; set; }
    string _dialogId = "editField";
    string updated_key = "PIN";
    string updated_value = "1234";

    public FieldEditTests()
    {
        TestField = new("", "", false);
    }

    void OnSaveClicked(string key, string value)
    {
        TestField.Key = key;
        TestField.Value = value;

        Debug.WriteLine($"FieldNew: OnSaveClicked(key={TestField.Key}, value={TestField.Value}, type={TestField.IsProtected})");
    }

    [Fact]
    public void Edit_Existing_Field() 
    {
        // Arrange
        IsNewField = false;
        var cut = Render(GetComponent());
        // Act
        cut.Find("textarea").Change(updated_value);
        cut.Find("button[type=submit]").Click();
        // Assert
        Assert.Equal(updated_value, TestField.Value);
    }

    [Fact]
    public void Edit_New_Field()
    {
        // Arrange
        IsNewField = true;
        var cut = Render(GetComponent());
        // Act
        cut.Find("#flexCheckDefault").Change(true);
        cut.Find("input").Change(updated_key);
        cut.Find("textarea").Change(updated_value);
        cut.Find("button[type=submit]").Click();
        // Assert
        Assert.True(TestField.IsProtected);
        Assert.Equal(updated_key, TestField.Key);
        Assert.Equal(updated_value, TestField.EditValue);
    }
}
