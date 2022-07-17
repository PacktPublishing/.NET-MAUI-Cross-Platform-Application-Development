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

        private string newKey = string.Empty;
        private string newValue = string.Empty;
        private bool IsPassword = false;
        private Field? listGroupField = default!;
        private string listGroupFieldKey => ((listGroupField != null) ? listGroupField.Key : "");
        private string listGroupFieldValue
        { 
            get 
            { 
                if (listGroupField == null) { return ""; }
                if (listGroupField.IsProtected) { return listGroupField.EditValue; }
                return listGroupField.Value; 
            }
        }
        bool IsKeyEditingEnable = false;

        public ItemDetail()
        {
            fields = new ObservableCollection<Field>();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (SelectedItemId != null)
            {
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
                    else 
                    {
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
            else 
            {
                throw new InvalidOperationException("ItemDetail: SelectedItemId is null");
            }
        }

        private void OnValueChanged(ChangeEventArgs e)
        {
            if (e.Value == null)
            {
                Debug.WriteLine("ItemDetail.OnValueChanged: ChangeEventArgs is null");
            }
            else
            {
                if (listGroupField == null || IsKeyEditingEnable)
                {
                    newValue = e.Value.ToString();
                    Debug.WriteLine($"ItemDetail.OnValueChanged: New Value={newValue}");
                }
                else
                {
                    listGroupField.Value = e.Value.ToString();
                    Debug.WriteLine($"ItemDetail.OnValueChanged: Value={listGroupField.Value}");
                }
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

        private async void UpdateFieldAsync(MouseEventArgs e)
        {
            if (selectedItem == null)
            {
                throw new NullReferenceException("Selected item is null");
            }

            if (listGroupField == null || IsKeyEditingEnable)
            {
                // Add a new field
                Field newField = selectedItem.AddField(newKey, newValue, IsPassword);
                fields.Add(newField);
                Debug.WriteLine($"ItemDetail.UpdateFieldAsync: New Key={newKey}, New Value={newValue}, IsPassword={IsPassword}");
            }
            else 
            {
                // Update the current field
                var newData = (listGroupField.IsProtected) ? listGroupField.EditValue : listGroupField.Value;
                selectedItem.UpdateField(listGroupField.Key, newData, listGroupField.IsProtected);
                Debug.WriteLine($"ItemDetail.UpdateFieldAsync: Key={listGroupField.Key}, Value={listGroupField.Value}");
            }
            await DataStore.UpdateItemAsync(selectedItem);
        }

        private async void DeleteFieldAsync(MouseEventArgs e)
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
