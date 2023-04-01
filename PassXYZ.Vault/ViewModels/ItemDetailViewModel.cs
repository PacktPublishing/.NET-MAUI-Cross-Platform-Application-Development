using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
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

        [ObservableProperty]
        private string? itemId;

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
