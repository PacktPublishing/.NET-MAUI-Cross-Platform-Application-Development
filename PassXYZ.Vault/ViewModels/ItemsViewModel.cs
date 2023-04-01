using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassXYZ.Vault.Models;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.Views;

namespace PassXYZ.Vault.ViewModels
{
    public partial class ItemsViewModel : ObservableObject
    {
        readonly IDataStore<Item> dataStore;
        private Item? _selectedItem = default;

        public ObservableCollection<Item> Items { get; }
        public Command<Item> ItemTapped { get; }

        public ItemsViewModel(IDataStore<Item> dataStore)
        {
            this.dataStore = dataStore;
            Title = "Browse";
            Items = new ObservableCollection<Item>();

            ItemTapped = new Command<Item>(OnItemSelected);
        }

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool isBusy;

        [RelayCommand]
        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        [RelayCommand]
        private async Task LoadItems()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await dataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                    Debug.WriteLine($"ItemsViewModel: {item.Name}, {item.Description}");
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
            IsBusy = true;
            SelectedItem = null;
            await LoadItems();
        }

        public Item? SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                if(value != null) 
                {
                    OnItemSelected(value);
                }
            }
        }

        public async void OnItemSelected(Item item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}