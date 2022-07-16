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

        private void UpdateField(MouseEventArgs e)
        {
            if (listGroupField == null || IsKeyEditingEnable)
            {
                Debug.WriteLine($"ItemDetail.UpdateField: New Key={newKey}, New Value={newValue}, IsPassword={IsPassword}");
            }
            else 
            {
                Debug.WriteLine($"ItemDetail.UpdateField: Key={listGroupField.Key}, Value={listGroupField.Value}");
            }
        }

        private void DeleteField(MouseEventArgs e)
        {
            if (listGroupField == null) return;
            Debug.WriteLine($"ItemDetail.DeleteField: Key={listGroupField.Key}, Value={listGroupField.Value}");
        }

    }
}
