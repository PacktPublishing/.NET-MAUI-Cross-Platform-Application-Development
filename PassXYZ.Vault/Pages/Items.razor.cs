using Microsoft.AspNetCore.Components;
using System.Diagnostics;

using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Services;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.ObjectModel;

namespace PassXYZ.Vault.Pages;

public partial class Items
{
    [Parameter]
    public string SelectedItemId { get; set; } = default!;

    [Inject]
    public IDataStore<Item> DataStore { get; set; } = default!;

    public string Title { get; set; } = default!;

    readonly ObservableCollection<Item> items;
    Item? selectedItem = default!;

    private string newItemTitle = string.Empty;
    private string newItemNotes = string.Empty;
    private string newItemType = ItemSubType.Entry.ToString();
    Item? listGroupItem = default!;
    private string listGroupItemName => (listGroupItem != null) ? listGroupItem.Name : "";
    private string listGroupItemNotes => (listGroupItem != null) ? listGroupItem.Notes : "";
    bool IsKeyEditingEnable = false;

    public Items() 
    {
        items = new ObservableCollection<Item>();
    }

    async Task LoadGroup(Item group)
    {
        if (group == null)
        {
            throw new ArgumentNullException("group");
        }
        DataStore.CurrentGroup = group;
        Title = DataStore.CurrentGroup.Name;
        items.Clear();
        var itemList = await DataStore.GetItemsAsync(true);
        foreach (var item in itemList)
        {
            // item.SetAvatar();
            items.Add(item);
        }
        Debug.WriteLine($"Items: Selected group is {DataStore.CurrentGroup.Name}");
    }

    /// <summary>
    /// We need to process query parameter here. There are three cases:
    /// 1. Case 1: Without parameter, this is the root group
    /// 2. Case 2: With parameter SelectedItemId and it is a group
    /// 3. Case 3: With parameter SelectedItemId and it is an entry
    /// </summary>
    protected override async void OnParametersSet()
    {
        base.OnParametersSet();

        if (SelectedItemId != null)
        {
            selectedItem = DataStore.GetItem(SelectedItemId, true);
            if (selectedItem == null)
            {
                Debug.WriteLine($"Items: Item cannot be found.");
                throw new ArgumentNullException("SelectedItemId");
            }
            else 
            {
                if (selectedItem.IsGroup)
                {
                    // Case 2: set to the current group
                    await LoadGroup(selectedItem);
                }
                else
                {
                    // Case 3: it is an entry
                    Debug.WriteLine($"Items: Selected entry is {selectedItem.Name}");
                    throw new InvalidOperationException("Items: Selected item must be group here.");
                }
            }
        }
        else 
        {
            // Case 1: Set to Root Group
            await LoadGroup(DataStore.RootGroup);
        }
    }

    private void OnNameChanged(ChangeEventArgs e)
    {
        if (e.Value == null)
        {
            Debug.WriteLine("Items.OnNameChanged: ChangeEventArgs is null");
        }
        else 
        {
            if (listGroupItem == null) return;

            listGroupItem.Name = e.Value.ToString();
            Debug.WriteLine($"Items.OnNameChanged: Notes={listGroupItem.Name}");
        }
    }

    private void OnNotesChanged(ChangeEventArgs e) 
    {
        if (e.Value == null) 
        {
            Debug.WriteLine("Items.OnNotesChanged: ChangeEventArgs is null");
        }
        else 
        {
            if (listGroupItem == null || IsKeyEditingEnable) 
            {
                newItemNotes = e.Value.ToString();
                Debug.WriteLine($"Items.OnNotesChanged: New Notes={newItemNotes}");
            }
            else 
            {
                listGroupItem.Notes = e.Value.ToString();
                Debug.WriteLine($"Items.OnNotesChanged: Notes={listGroupItem.Notes}");
            }
        }
    }

    private async void UpdateItemAsync(MouseEventArgs e)
    {
        if (listGroupItem == null || IsKeyEditingEnable) 
        {
            // Add new item
            var newType = ItemSubType.None;
            newType = newType.GetItemSubType(newItemType);
            Item? newItem = DataStore.CreateNewItem(newType);
            if (newItem != null)
            {
                newItem.Name = newItemTitle;
                newItem.Notes = newItemNotes;
                items.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            }
            Debug.WriteLine($"Items.AddNewItem: type={newItemType}, name={newItemTitle}, Notes={newItemNotes}");
        }
        else 
        {
            // Update the current item
            await DataStore.UpdateItemAsync(listGroupItem);
            Debug.WriteLine($"Items.UpdateItem: name={listGroupItem.Name}, Notes={listGroupItem.Notes}");
        }
    }

    private async void DeleteItemAsync(MouseEventArgs e)
    {
        if (listGroupItem == null) return;

        if (items.Remove(listGroupItem)) 
        {
            _ = await DataStore.DeleteItemAsync(listGroupItem.Id);
        }
        Debug.WriteLine($"Items.DeleteItem: name={listGroupItem.Name}, Notes={listGroupItem.Notes}");
    }
}