using System.Collections.Generic;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using KPCLib;
using KeePassLib;
using PassXYZLib;
using PassXYZ.Vault.Services;

namespace PassXYZ.Vault.ViewModels;

[QueryProperty(nameof(ItemId), nameof(ItemId))]
public partial class ItemDetailViewModel : BaseViewModel
{
    readonly IDataStore<Item> dataStore;
    ILogger<ItemDetailViewModel> logger;
    public ObservableCollection<Field> Fields { get; set; }

    public ItemDetailViewModel(IDataStore<Item> dataStore, ILogger<ItemDetailViewModel> logger)
    {
        this.dataStore = dataStore;
        this.logger = logger;
        Fields = new ObservableCollection<Field>();
    }

    [ObservableProperty]
    private string? title;

    [ObservableProperty]
    private string? id;

    [ObservableProperty]
    private string? description;

    [ObservableProperty]
    private bool isBusy;

        private string? itemId;
        public string ItemId
        {
            get
            {
                if(itemId == null) { throw new NullReferenceException(nameof(itemId)); }
                return itemId;
            }

            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

    public override void OnItemSelecteion(object sender) 
    {
        logger.LogDebug("OnItemSelecteion is invoked.");
    }

    public void LoadItemId(string itemId)
    {
        if (itemId == null) { throw new ArgumentNullException(nameof(itemId)); }
        var item = dataStore.GetItem(itemId);
        if (item == null) { throw new NullReferenceException(itemId); }
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
            logger.LogDebug($"ItemDetailViewModel: Name={dataEntry.Name}.");
        }
    }
}
