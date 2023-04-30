using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.Views;

namespace PassXYZ.Vault.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class ItemsViewModel : BaseViewModel
    {
        readonly IDataStore<Item> dataStore;
        ILogger<ItemsViewModel> logger;

        public ObservableCollection<Item> Items { get; }

        public ItemsViewModel(IDataStore<Item> dataStore, ILogger<ItemsViewModel> logger)
        {
            this.dataStore = dataStore;
            this.logger = logger;
            Title = "Browse";
            Items = new ObservableCollection<Item>();
            IsBusy = false;
        }

        [ObservableProperty]
        private Item? selectedItem = default;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool isBusy;

        [RelayCommand]
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
            }
            else
            {
                type = ItemSubType.None;
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

        public override async void OnItemSelecteion(object sender)
        {
            Item? item = sender as Item;
            if (item == null)
            {
                logger.LogWarning("item is null.");
                return;
            }

            if (item.IsGroup)
            {
                await Shell.Current.GoToAsync($"{nameof(ItemsPage)}?{nameof(ItemsViewModel.ItemId)}={item.Id}");
            }
            else
            {
                await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
            }
        }

        [RelayCommand]
        private async Task LoadItems()
        {
            try
            {
                Items.Clear();
                var items = await dataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
                logger.LogDebug($"IsBusy={IsBusy}, added {Items.Count()} items");
            }
            catch (Exception ex)
            {
                logger.LogError("{ex}", ex);
            }
            finally
            {
                IsBusy = false;
                logger.LogDebug("Set IsBusy to false");
            }
        }

        public string ItemId
        {
            get
            {
                return selectedItem == null ? string.Empty : selectedItem.Id;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    selectedItem = null;
                }
                else
                {
                    var item = dataStore.GetItem(value);
                    if (item != null)
                    {
                        selectedItem = item;
                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(ItemId), "cannot find the selected item");
                    }
                }
            }
        }

        /// <summary>
        /// The logic of navigation is implemented here.
        /// The current group is set here according to the selected item.
        /// </summary>
        public void OnAppearing()
        {
            if (SelectedItem == null)
            {
                // If SelectedItem is null, this is the root group.
                Title = dataStore.SetCurrentGroup();
            }
            else
            {
                Title = dataStore.SetCurrentGroup(selectedItem);
            }
            // load items
            IsBusy = true;
        }
    }
}