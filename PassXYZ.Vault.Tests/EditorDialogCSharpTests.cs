using PassXYZ.BlazorUI;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Reflection;

using PassXYZ.Vault.Tests.Targets;
using KPCLib;

namespace PassXYZ.Vault.Tests
{
    /// <summary>
    /// These tests are written entirely in C#.
    /// Learn more at https://bunit.dev/docs/getting-started/writing-tests.html#creating-basic-tests-in-cs-files
    /// </summary>
    public class EditorDialogCSharpTests : TestContext
    {
        [Fact]
        public void EditorDialogInitTest()
        {
            var cut = RenderComponent<EditorDialog>();
            cut.Find("h5").TextContent.MarkupMatches("editModal");
            Debug.WriteLine($"{cut.Markup}");
        }

        [Fact]
        public void EditorDialogEnableKeyEditTest() 
        {
            var cut = RenderComponent<EditorDialog>(parameters => parameters.Add(p => p.IsKeyEditingEnable, true));
            cut.Find("h5").TextContent.MarkupMatches("editModal");
            Debug.WriteLine($"{cut.Markup}");
        }

        [Fact]
        public void ItemNewTest()
        {
            TestItem testItem = new TestItem()
            {
                Name = "GitHub",
                Notes = "Your GitHub account",
                ItemType = "PxEntry"
            };
            var cut = RenderComponent<ItemNew>(
                parameters => parameters.Add(p => p.IsNewItem, true)
                .Add(p => p.testItem, testItem));
            cut.Find("h5").TextContent.MarkupMatches("Creating a new Item");
            //cut.Find("input").Change("GitHub");
            //cut.Find("textarea").Change("Your GitHub account");
            //cut.Find("select").Change("PxEntry");
            cut.Find("button[type=submit]").Click();
            Debug.WriteLine($"{cut.Markup}");
        }

        [Fact]
        public void ItemEditTest()
        {
            TestItem testItem = new TestItem()
            {
                Name = "Blazor",
                Notes = "Blazor unit testing"
            };
            var cut = RenderComponent<ItemNew>(
                parameters => parameters.Add(p => p.IsNewItem, false)
                .Add(p => p.testItem, testItem));
            cut.Find("h5").TextContent.MarkupMatches("Creating a new Item");
            //cut.Find("textarea").Change("This is the new value.");
            cut.Find("button[type=submit]").Click();
            Debug.WriteLine($"{cut.Markup}");
        }

        [Fact]
        public void FieldNewTest() 
        {
            Field testField = new("UserName", "John Yang", false);
            var cut = RenderComponent<FieldNew>(
                parameters => parameters.Add(p => p.IsNewField, true)
                .Add(p => p.TestField, testField));
            cut.Find("h5").TextContent.MarkupMatches("Creating a new Field");
            //cut.Find("input").Change("Username");
            //cut.Find("textarea").Change("John Yang");
            //cut.Find("input[type=\"checkbox\"]").Change("false");
            cut.Find("button[type=submit]").Click();
            Debug.WriteLine($"{cut.Markup}");
        }

        [Fact]
        public void FieldEditTest() 
        {
            Field testField = new("PIN", "1234567890", true);
            var cut = RenderComponent<FieldNew>(
                parameters => parameters.Add(p => p.IsNewField, false)
                .Add(p => p.TestField, testField));
            cut.Find("h5").TextContent.MarkupMatches("Creating a new Field");
            //cut.Find("textarea").Change("1234567890");
            //cut.Find("input[type=\"checkbox\"]").Change("false");
            cut.Find("button[type=submit]").Click();
            Debug.WriteLine($"{cut.Markup}");
        }
    }
}