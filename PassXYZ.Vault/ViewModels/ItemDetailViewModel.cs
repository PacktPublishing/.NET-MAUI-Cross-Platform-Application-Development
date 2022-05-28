using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using KPCLib;
using KeePassLib;
using PassXYZLib;
using PassXYZ.Vault.Properties;
using PassXYZ.Vault.Views;

namespace PassXYZ.Vault.ViewModels;

[QueryProperty(nameof(ItemId), nameof(ItemId))]
public class ItemDetailViewModel : BaseViewModel
{
    private Item? _item { get; set; }
    public ObservableCollection<Field> Fields { get; set; }
    public Command LoadFieldsCommand { get; }
    public Command AddFieldCommand { get; }
    public Command AddBinaryCommand { get; }
    public Command<Field> FieldTapped { get; }

    private string? description = default;
    public string? Description
    {
        get => description;
        set => SetProperty(ref description, value);
    }

    private string? itemId = default;
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
        LoadFieldsCommand = new Command(() => ExecuteLoadFieldsCommand());
        FieldTapped = new Command<Field>(OnFieldSelected);
        AddFieldCommand = new Command(OnAddField);
        AddBinaryCommand = new Command(OnAddBinary);
    }

    private void ExecuteLoadFieldsCommand()
    {
        try
        {
            if (_item != null)
            {
                Fields.Clear();
                List<Field> fields = _item.GetFields();
                foreach (Field field in fields)
                {
                    Fields.Add(field);
                }
                Debug.WriteLine($"ItemDetailViewModel: (LFC) Name={_item.Name}, IsBusy={IsBusy}.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ItemDetailViewModel: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void LoadItemId(string itemId)
    {
        try
        {
            var item = await DataStore.GetItemAsync(itemId);
            if (item == null)
            {
                throw new ArgumentNullException(nameof(itemId));
            }

            Title = item.Name;
            Description = item.GetNotesInHtml();
            _item = item;
            Fields.Clear();
            List<Field> fields = _item.GetFields();
            foreach (Field field in fields)
            {
                Fields.Add(field);
            }
            Debug.WriteLine($"ItemDetailViewModel: Name={_item.Name}, IsBusy={IsBusy}.");
        }
        catch (Exception)
        {
            Debug.WriteLine("Failed to Load Item");
        }
    }

    /// <summary>
    /// Update a field.
    /// </summary>
    /// <param name="field">an instance of Field</param>
    public async void Update(Field field)
    {
        if (field == null)
        {
            return;
        }

        if (!field.IsBinaries && _item != null)
        {
            await Shell.Current.Navigation.PushAsync(new FieldEditPage(async (string k, string v, bool isProtected) => {
                _item.UpdateField(k, v, field.IsProtected);
                await DataStore.UpdateItemAsync(_item);
            }, field.Key, field.EditValue));
        }
        else
        {
            await Shell.Current.DisplayAlert(Properties.Resources.label_id_attachment, Properties.Resources.message_id_edit_binary, Properties.Resources.alert_id_ok);
        }
    }
    /// <summary>
    /// Delete a field.
    /// </summary>
    /// <param name="field">an instance of Field</param>
    public async void DeletedAsync(Field field) 
    {
        if (field == null)
        {
            return;
        }

        if (Fields.Remove(field) && _item != null)
        {
            _item.DeleteField(field);
            await DataStore.UpdateItemAsync(_item);
        }
        else
        {
            return;
        }
    }
    private async void OnAddField(object obj)
    {
        await Shell.Current.Navigation.PushAsync(new FieldEditPage(async (string k, string v, bool isProtected) => {
            Field field = _item.AddField(k, v, isProtected);
            if (field != null && _item != null)
            {
                Fields.Add(field);
                await DataStore.UpdateItemAsync(_item);
            }
        }, _item));
    }

    private async void AddBinary(string tempFilePath, string fileName)
    {
        if (_item != null) 
        {
            var vBytes = File.ReadAllBytes(tempFilePath);
            Field field = _item.AddBinaryField(fileName, vBytes, Properties.Resources.label_id_attachment);
            if(field != null) 
            {
                Fields.Add(field);
                await DataStore.UpdateItemAsync(_item);
            }
        }
    }

    private async Task LoadPhotoAsync(FileResult photo)
    {
        // canceled
        if (photo == null)
        {
            return;
        }
        // save the file into local storage
        var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
        using (var stream = await photo.OpenReadAsync())
        using (var newStream = File.OpenWrite(newFile))
            await stream.CopyToAsync(newStream);
        AddBinary(newFile, photo.FileName);
    }

    private async void OnAddBinary(object obj)
    {
        List<string> inputTypeList = new List<string>()
            {
                Properties.Resources.field_id_file,
                Properties.Resources.field_id_camera,
                Properties.Resources.field_id_gallery
            };


        var typeValue = await Shell.Current.DisplayActionSheet(Properties.Resources.message_id_attachment_options, Properties.Resources.action_id_cancel, null, inputTypeList.ToArray());
        if (typeValue == Properties.Resources.field_id_gallery)
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                await LoadPhotoAsync(photo);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
                Debug.WriteLine($"ItemDetailViewModel: PickPhotoAsync => {fnsEx.Message}");
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
                Debug.WriteLine($"ItemDetailViewModel: PickPhotoAsync => {pEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ItemDetailViewModel: PickPhotoAsync => {ex.Message}");
            }
            Debug.WriteLine($"ItemDetailViewModel: Add an attachment from Gallery");
        }
        else if (typeValue == Properties.Resources.field_id_file)
        {
            try
            {
                var result = await FilePicker.PickAsync();
                if (result != null)
                {
                    var tempFilePath = Path.GetTempFileName();
                    var stream = await result.OpenReadAsync();
                    var fileStream = File.Create(tempFilePath);
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                    fileStream.Close();
                    AddBinary(tempFilePath, result.FileName);
                    File.Delete(tempFilePath);
                }
                else
                {
                    await Shell.Current.DisplayAlert(Properties.Resources.message_id_attachment_options, Properties.Resources.import_error_msg, Properties.Resources.alert_id_ok);
                }
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
                Debug.WriteLine($"LoginViewModel: Import attachment, {ex}");
            }
            Debug.WriteLine($"ItemDetailViewModel: Add an attachment from local storage");
        }
        else if (typeValue == Properties.Resources.field_id_camera)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                await LoadPhotoAsync(photo);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
                Debug.WriteLine($"ItemDetailViewModel: CapturePhotoAsync => {fnsEx.Message}");
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
                Debug.WriteLine($"ItemDetailViewModel: CapturePhotoAsync => {pEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ItemDetailViewModel: CapturePhotoAsync => {ex.Message}");
            }
            Debug.WriteLine($"ItemDetailViewModel: Add an attachment using Camera");
        }
    }

    private async void OnFieldSelected(Field field)
    {
        if (field == null)
        {
            return;
        }

        if (field.IsBinaries)
        {
            var bdc = BinaryDataClassifier.ClassifyUrl(field.Key);
            if ((bdc == BinaryDataClass.Image) && (field.Binary != null))
            {
                await Shell.Current.Navigation.PushAsync(new ImagePreviewPage(field.GetBinaryData()));
            }
            else
            {
                //PassXYZ.Utils.BinaryDataUtil.Open(field.Key, field.Binary, null);
                Debug.WriteLine($"ItemDetailViewModel: Attachment {field.Key} selected");
            }
        }
    }
}
