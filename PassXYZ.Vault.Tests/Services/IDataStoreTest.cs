using PassXYZ.Vault.Models;
using PassXYZ.Vault.Services;

namespace PassXYZ.Vault.Tests.Services
{
    public class IDataStoreTest
    {
        public IDataStoreTest() { }

        [Fact]
        public async void GetItemsAsyncTest() 
        {
            IDataStore<Item> dataStore = new MockDataStore();
            var items = await dataStore.GetItemsAsync(true);
            Assert.Equal(6, items.Count());
        }

        [Fact]
        public async void AddItemAsyncTest()
        {
            IDataStore<Item> dataStore = new MockDataStore();
            Item newItem = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "New item 1",
                Description = "This is a new item."
            };
            var result = await dataStore.AddItemAsync(newItem);
            Assert.True(result);
        }

        [Fact]
        public async void AddItemAsyncFailureTest() 
        {
            IDataStore<Item> dataStore = new MockDataStore();
            bool result = false;
#pragma warning disable CS8625 // Possible null reference argument.
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => result = await dataStore.AddItemAsync(null));
#pragma warning restore CS8625 // Possible null reference argument.
            Assert.Equal("Value cannot be null. (Parameter 'item')", ex.Message);
        }

        [Fact]
        public async void UpdateItemAsyncTest()
        {
            IDataStore<Item> dataStore = new MockDataStore();
            Item newItem = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "New item 1",
                Description = "This is a new item."
            };
            var result = await dataStore.AddItemAsync(newItem);
            Assert.True(result);
            newItem.Name = "Updated item 1";
            result = await dataStore.UpdateItemAsync(newItem);
            Assert.True(result);
        }

        [Fact]
        public async void UpdateNullItemAsyncTest() 
        {
            IDataStore<Item> dataStore = new MockDataStore();
            bool result = false;
#pragma warning disable CS8625 // Possible null reference argument.
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => result = await dataStore.UpdateItemAsync(null));
#pragma warning restore CS8625 // Possible null reference argument.
            Assert.Equal("Value cannot be null. (Parameter 'item')", ex.Message);
        }

        [Fact]
        public async void UpdateNoExistItemAsyncTest() 
        {
            IDataStore<Item> dataStore = new MockDataStore();
            Item newItem = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "No item 1",
                Description = "You cannot find this item."
            };
            var result = await dataStore.UpdateItemAsync(newItem);
            Assert.False(result);
        }

        [Fact]
        public async void DeleteItemAsyncTest()
        {
            IDataStore<Item> dataStore = new MockDataStore();
            Item newItem = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "New item 1",
                Description = "Please delete it."
            };
            var result = await dataStore.AddItemAsync(newItem);
            Assert.True(result);
            result = await dataStore.DeleteItemAsync(newItem.Id);
            Assert.True(result);
        }

        [Fact]
        public async void DeleteNullItemAsyncTest()
        {
            IDataStore<Item> dataStore = new MockDataStore();
            bool result = false;
#pragma warning disable CS8625 // Possible null reference argument.
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => result = await dataStore.DeleteItemAsync(null));
#pragma warning restore CS8625 // Possible null reference argument.
            Assert.Equal("Value cannot be null. (Parameter 'id')", ex.Message);
        }

        [Fact]
        public async void DeleteNoExistItemAsyncTest()
        {
            IDataStore<Item> dataStore = new MockDataStore();
            Item newItem = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "No item 1",
                Description = "You cannot find this item."
            };
            var result = await dataStore.DeleteItemAsync(newItem.Id);
            Assert.False(result);
        }

        [Fact]
        public async void GetItemAsyncTest()
        {
            IDataStore<Item> dataStore = new MockDataStore();
            Item newItem = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "New item 1",
                Description = "This is a new item."
            };
            var result = await dataStore.AddItemAsync(newItem);
            Assert.True(result);
            var item = await dataStore.GetItemAsync(newItem.Id);
            Assert.NotNull(item);
            Assert.Equal(newItem.Name, item.Name);
        }

        [Fact]
        public async void GetNullItemAsyncTest()
        {
            IDataStore<Item> dataStore = new MockDataStore();
#pragma warning disable CS8625 // Possible null reference argument.
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => await dataStore.GetItemAsync(null));
#pragma warning restore CS8625 // Possible null reference argument.
            Assert.Equal("Value cannot be null. (Parameter 'id')", ex.Message);
        }

        [Fact]
        public async void GetNoExistItemAsyncTest()
        {
            IDataStore<Item> dataStore = new MockDataStore();
            Item newItem = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "No item 1",
                Description = "You cannot find this item."
            };
            var result = await dataStore.GetItemAsync(newItem.Id);
            Assert.Null(result);
        }
    }
}
