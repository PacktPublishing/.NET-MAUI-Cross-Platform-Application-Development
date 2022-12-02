using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.ObjectModel;
using System.Diagnostics;

using KPCLib;
using KeePassLib;
using PassXYZLib;
using PassXYZ.Vault.Shared;
using PassXYZ.Vault.Services;

namespace PassXYZ.Vault.Pages
{
    public partial class ItemDetail 
    {
        [Parameter]
        public string SelectedItemId { get; set; } = default!;

        string? notes = default!;

        [Inject]
        public IDataStore<Item> DataStore { get; set; } = default!;

        readonly ObservableCollection<Field> fields;

        Item? selectedItem = default!;

        public ItemDetail()
        {
            fields = new ObservableCollection<Field>();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (SelectedItemId == null)
            {
                throw new InvalidOperationException("ItemDetail: SelectedItemId is null");
            }

            selectedItem = DataStore.GetItem(SelectedItemId, true);
            if (selectedItem == null)
            {
                throw new InvalidOperationException("ItemDetail: entry cannot be found with SelectedItemId");
            }
            else
            {
                if (selectedItem.IsGroup)
                {
                    throw new InvalidOperationException("ItemDetail: SelectedItemId should not be group here.");
                }

                fields.Clear();
                List<Field> tmpFields = selectedItem.GetFields();
                foreach (Field field in tmpFields)
                {
                    fields.Add(field);
                    //field.ImgSource = field.SetAvatar("Icon", "file");
                }
                notes = selectedItem.GetNotesInHtml();
                Debug.WriteLine($"ItemDetail: Item name is {selectedItem.Name}.");
            }
        }
    }
}
