using Microsoft.AspNetCore.Components;
using System.Diagnostics;

using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.ViewModels;
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

    public Items() 
    {
        items = new ObservableCollection<Item>();
    }

    async Task LoadGroup(Item group)
    {
        Title = DataStore.SetCurrentGroup(group);
        items.Clear();
        var itemList = await DataStore.GetItemsAsync(true);
        foreach (var item in itemList)
        {
            // item.SetAvatar();
            items.Add(item);
        }
        Debug.WriteLine($"Items: Selected group is {Title}");
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
            selectedItem = DataStore.GetItem(SelectedItemId);
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
            await LoadGroup(null);
        }
    }
}