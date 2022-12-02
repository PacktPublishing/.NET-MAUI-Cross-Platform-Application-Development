using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.ObjectModel;
using System.Diagnostics;

using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.Shared;
using PassXYZ.BlazorUI;

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

    NewItem _newItem;
    Item listGroupItem;
    KeyValueData<Item> currentItem;
    bool _isNewItem = false;
    string _dialogEditId = "editItem";
    string _dialogDeleteId = "deleteItem";

    public Items() 
    {
        listGroupItem = _newItem = new();
        currentItem = new()
        {
            Data = listGroupItem
        };

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
        else 
        {
            // Case 1: Set to Root Group
            await LoadGroup(DataStore.RootGroup);
        }
    }

    private async Task<bool> UpdateItemAsync(string key, string value)
    {
        if (listGroupItem == null) return false;
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) return false;

        listGroupItem.Name = key;
        listGroupItem.Notes = value;

        if (_isNewItem)
        {
            // Add new item
            if (listGroupItem is NewItem aNewItem)
            {
                Item? newItem = DataStore.CreateNewItem(aNewItem.SubType);
                if (newItem != null)
                {
                    newItem.Name = aNewItem.Name;
                    newItem.Notes = aNewItem.Notes;
                    items.Add(newItem);
                    await DataStore.AddItemAsync(newItem);
                }
                Debug.WriteLine($"Items.AddNewItem: type={aNewItem.ItemType}, name={aNewItem.Name}, Notes={aNewItem.Notes}");
            }
        }
        else 
        {
            // Update the current item
            await DataStore.UpdateItemAsync(listGroupItem);
            Debug.WriteLine($"Items.UpdateItem: name={listGroupItem.Name}, Notes={listGroupItem.Notes}");
        }
        StateHasChanged();
        return true;
    }

    private async void DeleteItemAsync()
    {
        if (listGroupItem == null) return;

        if (items.Remove(listGroupItem)) 
        {
            _ = await DataStore.DeleteItemAsync(listGroupItem.Id);
        }
        Debug.WriteLine($"Items.DeleteItem: name={listGroupItem.Name}, Notes={listGroupItem.Notes}");
    }
}