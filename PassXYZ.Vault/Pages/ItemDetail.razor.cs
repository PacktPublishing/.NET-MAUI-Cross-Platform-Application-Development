using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.ObjectModel;
using System.Diagnostics;

using KPCLib;
using KeePassLib;
using PassXYZLib;
using PassXYZ.Vault.Shared;
using PassXYZ.Vault.Services;
using PassXYZ.BlazorUI;

namespace PassXYZ.Vault.Pages
{
    public partial class ItemDetail 
    {
        [Parameter]
        public string SelectedItemId { get; set; } = default!;

        [Inject]
        public IDataStore<Item> DataStore { get; set; } = default!;

        readonly ObservableCollection<Field> fields;
        Item? selectedItem = default!;
        private Field _newField;
        private Field listGroupField;
        KeyValueData<Field> currentField;
        bool _isNewField = false;
        string? notes = default!;
        string _dialogEditId = "editField";
        string _dialogDeleteId = "deleteField";

        public ItemDetail()
        {
            listGroupField = _newField = new("","",false);
            currentField = new()
            {
                Data = listGroupField
            };
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

        private void OnToggleShowPassword(MouseEventArgs e) 
        {
            if (listGroupField == null) return;

            if (listGroupField.IsHide)
            {
                listGroupField.ShowPassword();
            }
            else 
            {
                listGroupField.HidePassword();
            }
        }

        private async Task<bool> UpdateFieldAsync(string key, string value)
        {
            if (selectedItem == null || listGroupField == null)
            {
                throw new NullReferenceException("Selected item is null");
            }
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) return false;
            listGroupField.Key = key;
            listGroupField.Value = value;

            if (_isNewField)
            {
                // Add a new field
                Field newField = selectedItem.AddField(listGroupField.Key, 
                    ((listGroupField.IsProtected) ? listGroupField.EditValue : listGroupField.Value), 
                    listGroupField.IsProtected);
                fields.Add(newField);
                Debug.WriteLine($"ItemDetail.UpdateFieldAsync: New Key={listGroupField.Key}, New Value={listGroupField.Value}, IsPassword={listGroupField.IsProtected}");
            }
            else 
            {
                // Update the current field
                var newData = (listGroupField.IsProtected) ? listGroupField.EditValue : listGroupField.Value;
                selectedItem.UpdateField(listGroupField.Key, newData, listGroupField.IsProtected);
                Debug.WriteLine($"ItemDetail.UpdateFieldAsync: Key={listGroupField.Key}, Value={listGroupField.Value}");
            }
            await DataStore.UpdateItemAsync(selectedItem);
            StateHasChanged();
            return true;
        }

        private async void DeleteFieldAsync()
        {
            if (listGroupField == null || selectedItem == null) 
            {
                throw new NullReferenceException("Selected item or field is null");
            }
            listGroupField.ShowContextAction = listGroupField;
            selectedItem.DeleteField(listGroupField);
            await DataStore.UpdateItemAsync(selectedItem);
            Debug.WriteLine($"ItemDetail.DeleteFieldAsync: Key={listGroupField.Key}, Value={listGroupField.Value}");
        }

    }
}
