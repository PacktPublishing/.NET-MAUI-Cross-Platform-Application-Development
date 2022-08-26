using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Services;

namespace PassXYZ.Vault.Tests
{
    public class DataStoreTests
    {
        IDataStore<Item> datastore;

        public DataStoreTests() 
        {
            datastore = new MockDataStore();
        }

        [Fact]
        public async void Add_Item() 
        { 
            // Arrange
            ItemSubType itemSubType = ItemSubType.Entry;

            // Act
            var newItem = datastore.CreateNewItem(itemSubType);
            newItem.Name = $"{itemSubType.ToString()}01";
            await datastore.AddItemAsync(newItem);
            var item = datastore.GetItem(newItem.Id);

            // Assert
            Assert.Equal(newItem.Id, item.Id);
        }

        [Theory]
        [InlineData(ItemSubType.Entry)]
        [InlineData(ItemSubType.Group)]
        [InlineData(ItemSubType.Notes)]
        [InlineData(ItemSubType.PxEntry)]
        public async void Delete_Item(ItemSubType itemSubType) 
        {
            // Arrange
            var newItem = datastore.CreateNewItem(itemSubType);
            newItem.Name = $"{itemSubType.ToString()}01";
            await datastore.AddItemAsync(newItem);

            // Act
            bool result = await datastore.DeleteItemAsync(newItem.Id);
            Debug.WriteLine($"Delete_Item: {newItem.Name}");

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(ItemSubType.Entry)]
        [InlineData(ItemSubType.Group)]
        [InlineData(ItemSubType.Notes)]
        [InlineData(ItemSubType.PxEntry)]
        public void Create_Item(ItemSubType itemSubType) 
        {
            var item = datastore.CreateNewItem(itemSubType);
            item.Name = itemSubType.ToString();
            Debug.WriteLine($"Create_Item: {item.Name}");

            Assert.NotNull(item);
        }
    }
}
