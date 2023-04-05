using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using PassXYZ.Vault.Models;
using PassXYZ.Vault.Services;

namespace PassXYZ.Vault.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class ItemDetailViewModel : ObservableObject
    {
        readonly IDataStore<Item> dataStore;
        ILogger<ItemDetailViewModel> logger;

        public ItemDetailViewModel(IDataStore<Item> dataStore, ILogger<ItemDetailViewModel> logger)
        {
            this.dataStore = dataStore;
            this.logger = logger;
        }

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private string? id;

        [ObservableProperty]
        private string? name;

        [ObservableProperty]
        private string? description;

        private string? itemId;
        public string ItemId
        {
            get => itemId;
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
        {
            if (itemId == null) { throw new ArgumentNullException(nameof(itemId)); }
            try
            {
                var item = await dataStore.GetItemAsync(itemId);
                if (item == null) { logger.LogDebug("cannot find {itemId}", itemId); return; }
                Id = item.Id;
                Name = item.Name;
                Description = item.Description;
            }
            catch (Exception)
            {
                logger.LogError("Failed to Load Item");
            }
        }
    }
}
