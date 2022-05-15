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
    private string? itemId = default;
    private string? description = default;
    public string? Id { get; set; }
    public ObservableCollection<Field> Fields { get; set; }

    public string? Description
    {
        get => description;
        set => SetProperty(ref description, value);
    }

    public string? ItemId
    {
        get
        {
            return itemId;
        }
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            itemId = value;
            LoadItemId(value);
        }
    }

    public ItemDetailViewModel()
    {
        Fields = new ObservableCollection<Field>();
        Id = default;
    }

    public async void LoadItemId(string itemId)
    {
        try
        {
            var item = await DataStore.GetItemAsync(itemId);
            if (item == null)
            {
                throw new ArgumentNullException(nameof(itemId));
            }

            Id = item.Id;
            Title = item.Name;
            Description = item.Description;
            PwEntry dataEntry = (PwEntry)item;
            Fields.Clear();
            List<Field> fields = dataEntry.GetFields(GetImage: FieldIcons.GetImage);
            foreach (Field field in fields)
            {
                Fields.Add(field);
            }
            Debug.WriteLine($"ItemDetailViewModel: Name={dataEntry.Name}, IsBusy={IsBusy}.");
        }
        catch (Exception)
        {
            Debug.WriteLine("Failed to Load Item");
        }
    }
}
