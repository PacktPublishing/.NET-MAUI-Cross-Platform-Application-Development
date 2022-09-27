using System.Collections.ObjectModel;
using System.Diagnostics;

using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Properties;
using PassXYZ.Vault.Views;

namespace PassXYZ.Vault.ViewModels;

[QueryProperty(nameof(ItemId), nameof(ItemId))]
public class ItemsViewModel : BaseViewModel
{
    private Item? _selectedItem = default;
    public ObservableCollection<Item> Items { get; }
    public Command LoadItemsCommand { get; }
    public Command AddItemCommand { get; }
    public Command<Item> ItemTapped { get; }

    public string ItemId
    {
        get
        {
            return _selectedItem == null ? string.Empty : _selectedItem.Id;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                var item = DataStore.GetItem(value, true);
                if (item != null) 
                {
                    _selectedItem = DataStore.CurrentGroup = item;
                    Debug.WriteLine($"ItemsViewModel: ItemId={DataStore.CurrentGroup!.Name}, {DataStore.CurrentGroup!.Description}");
                }
                else 
                {
                    Debug.WriteLine($"ItemsViewModel: ItemId cannot be found.");
                    throw new ArgumentNullException("ItemId");
                }
            }
            else 
            {
                _selectedItem = null;
                DataStore.CurrentGroup = DataStore.RootGroup;
            }
            ExecuteLoadItemsCommand();
        }
    }

    public ItemsViewModel()
    {
        Title = "Browse";
        Items = new ObservableCollection<Item>();
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

        ItemTapped = new Command<Item>(OnItemSelected);

        AddItemCommand = new Command(OnAddItem);
    }

    public async Task ExecuteLoadItemsCommand()
    {
        IsBusy = true;

        try
        {
            if (_selectedItem == null)
            {
                // This is the case for root group.
                DataStore.CurrentGroup = DataStore.RootGroup;
                Debug.WriteLine($"ItemsViewModel: loading {DataStore.CurrentGroup.Name}");
            }

            if (DataStore.RootGroup != null) 
            {
                Title = DataStore.CurrentGroup.Name;
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                Debug.WriteLine($"ItemsViewModel: loading from {DataStore.CurrentGroup.Name}");
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            else 
            {
                throw new ArgumentNullException("RootGroup is null");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    async public void OnAppearing()
    {
        //IsBusy = true;
        if (_selectedItem == null) 
        {
            // Loading from RootPage
            await ExecuteLoadItemsCommand();
        }
    }

    public Item? SelectedItem
    {
        get => _selectedItem;
        set
        {
            SetProperty(ref _selectedItem, value);
            if(value != null) 
            {
                Debug.WriteLine($"ItemsViewModel: SelectedItem is {_selectedItem.Name}");
                OnItemSelected(value);
            }
        }
    }

    private async void OnAddItem(object obj)
    {
        string[] templates = {
                Properties.Resources.item_subtype_group,
                Properties.Resources.item_subtype_entry,
                Properties.Resources.item_subtype_notes,
                Properties.Resources.item_subtype_pxentry
            };

        var template = await Shell.Current.DisplayActionSheet(Properties.Resources.pt_id_choosetemplate, Properties.Resources.action_id_cancel, null, templates);
        ItemSubType type;
        if (template == Properties.Resources.item_subtype_entry)
        {
            type = ItemSubType.Entry;
        }
        else if (template == Properties.Resources.item_subtype_pxentry)
        {
            type = ItemSubType.PxEntry;
        }
        else if (template == Properties.Resources.item_subtype_group)
        {
            type = ItemSubType.Group;
        }
        else if (template == Properties.Resources.item_subtype_notes)
        {
            type = ItemSubType.Notes;
        }
        else if (template == Properties.Resources.action_id_cancel)
        {
            type = ItemSubType.None;
            Debug.WriteLine("Canceled the Template selection.");
        }
        else
        {
            type = ItemSubType.None;
            Debug.WriteLine("Canceled the Template selection.");
        }

        if (type != ItemSubType.None)
        {
            var itemType = new Dictionary<string, object>
            {
                { "Type", type }
            };
            await Shell.Current.GoToAsync(nameof(NewItemPage), itemType);
        }
    }

    /// <summary>
    /// Update an item. The item can be a group or an entry.
    /// </summary>
    /// <param name="item">an instance of Item</param>
    public async void Update(Item item)
    {
        if (item == null)
        {
            return;
        }

        await Shell.Current.Navigation.PushAsync(new FieldEditPage(async (string k, string v, bool isProtected) => {
            item.Name = k;
            item.Notes = v;
            await DataStore.UpdateItemAsync(item);
        }, item.Name, item.Notes, true));
    }

    /// <summary>
    /// Delete an item.
    /// </summary>
    /// <param name="item">an instance of Item</param>
    public async Task DeletedAsync(Item item)
    {
        if (item == null)
        {
            return;
        }

        if (Items.Remove(item))
        {
            _ = await DataStore.DeleteItemAsync(item.Id);
        }
        else
        {
            return;
        }

    }

    public async void OnItemSelected(Item item)
    {
        if (item == null)
            return;

        if (item.IsGroup) 
        {
            // This will push the ItemsPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemsPage)}?{nameof(ItemsViewModel.ItemId)}={item.Id}");
        }
        else 
        {
            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}