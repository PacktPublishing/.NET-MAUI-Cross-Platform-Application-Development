using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using KPCLib;
using KeePassLib;
using PassXYZLib;

namespace PassXYZ.Vault.ViewModels;

[QueryProperty(nameof(ItemId), nameof(ItemId))]
public class ItemDetailViewModel : BaseViewModel
{
    private string itemId;
    private string description;
    public string Id { get; set; }
    public ObservableCollection<Field> Fields { get; set; }

    public string Description
    {
        get => description;
        set => SetProperty(ref description, value);
    }

    public string ItemId
    {
        get
        {
            return itemId;
        }
        set
        {
            itemId = value;
            LoadItemId(value);
        }
    }

    public ItemDetailViewModel()
    {
        Fields = new ObservableCollection<Field>();
    }

    public async void LoadItemId(string itemId)
    {
        try
        {
            var item = await DataStore.GetItemAsync(itemId);
            Id = item.Id;
            Title = item.Name;
            Description = item.Description;

            if (!item.IsGroup) 
            {
                PwEntry dataEntry = (PwEntry)item;
                Fields.Clear();
                List<Field> fields = dataEntry.GetFields(GetImage: FieldIcons.GetImage);
                foreach (Field field in fields)
                {
                    Fields.Add(field);
                }
                Debug.WriteLine($"ItemDetailViewModel: Name={dataEntry.Name}, IsBusy={IsBusy}.");
            }
        }
        catch (Exception)
        {
            Debug.WriteLine("Failed to Load Item");
        }
    }
}
