using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using KPCLib;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.ViewModels;
using System.Collections.ObjectModel;

namespace PassXYZ.Vault.Tests.ViewModels
{
    public class ItemDetailViewModelTests
    {
        ILogger<ItemDetailViewModel> logger;
        readonly IDataStore<Item> dataStore;

        public ItemDetailViewModelTests() 
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole()
                .SetMinimumLevel(LogLevel.Debug));
            logger = loggerFactory.CreateLogger<ItemDetailViewModel>();
            dataStore = new MockDataStore();
        }

        [Fact]
        public async void LoadItemIdTests()
        {
            // Arrange
            ItemDetailViewModel vm = new(dataStore, logger);
            var items = await dataStore.GetItemsAsync(true);
            foreach (var item in items)
            {
                // Act
                if (item == null) { throw new NullReferenceException(nameof(item)); };
#pragma warning disable CS8604 // Possible null reference argument.
                vm.LoadItemId(item.Id);
#pragma warning restore CS8604 // Possible null reference argument.
                // Assert
                Assert.Equal(item.Name, vm.Title);
            }
        }

        [Fact]
        public async void SetItemIdTests()
        {
            // Arrange
            ItemDetailViewModel vm = new(dataStore, logger);
            var items = await dataStore.GetItemsAsync(true);
            foreach (var item in items)
            {
                // Act
                vm.ItemId = item.Id;
                // Assert
                Assert.Equal(item.Name, vm.Title);
            }
        }
    }
}
