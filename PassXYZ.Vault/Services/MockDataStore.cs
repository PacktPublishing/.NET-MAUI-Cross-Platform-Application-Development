using PassXYZ.Vault.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PassXYZ.Vault.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> items;

        public MockDataStore()
        {
            items = new List<Item>();
            for (int i = 1; i <= 50; i++)
            {
                items.Add(new Item { Id = Guid.NewGuid().ToString(), Name = $"Item {i}", Description = $"This is the description for item {i}." });
            }
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            items.Add(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }

            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            if (oldItem != null) 
            {
                items.Remove(oldItem);
                items.Add(item);
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }

            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            if (oldItem != null) 
            {
                items.Remove(oldItem);
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<Item?> GetItemAsync(string id)
        {
            if(id == null) { throw new ArgumentNullException(nameof(id)); }

            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}