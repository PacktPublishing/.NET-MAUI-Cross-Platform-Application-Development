using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using KPCLib;

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
        private async Task AddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        public override async void OnItemSelecteion(object sender)
        {
            Item? item = sender as Item;
            if (item == null)
            {
                logger.LogWarning("item is null.");
                return;
            }
            logger.LogDebug($"Selected item is {item.Name}");
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
                return SelectedItem == null ? string.Empty : SelectedItem.Id;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    SelectedItem = null;
                }
                else
                {
                    var item = dataStore.GetItem(value);
                    if (item != null)
                    {
                        SelectedItem = item;
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
                Title = dataStore.SetCurrentGroup(SelectedItem);
            }
            // load items
            logger.LogDebug($"Loading group {Title}");
            IsBusy = true;
        }
    }
}