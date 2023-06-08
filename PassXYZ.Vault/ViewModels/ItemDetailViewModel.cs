using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using KPCLib;
using KeePassLib;
using PassXYZLib;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.Views;

namespace PassXYZ.Vault.ViewModels;

[QueryProperty(nameof(ItemId), nameof(ItemId))]
public partial class ItemDetailViewModel : BaseViewModel
{
    readonly IDataStore<Item> dataStore;
    ILogger<ItemDetailViewModel> logger;
    private Item? _item = default;
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

    public override async void OnSelection(object sender) 
    {
        Field? field = sender as Field;
        if (field == null)
        {
            logger.LogWarning("field is null in OnSelection");
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
                logger.LogDebug("Attachment {field.Key} selected", field.Key);
            }
        }
    }

    public void LoadItemId(string itemId)
    {
        if (itemId == null) { throw new ArgumentNullException(nameof(itemId)); }
        var item = dataStore.GetItem(itemId);
        if (item == null) { throw new NullReferenceException(itemId); }
        Id = item.Id;
        Title = item.Name;
        Description = item.GetNotesInHtml();
        _item = item;

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

    /// <summary>
    /// Update a field.
    /// </summary>
    /// <param name="field">an instance of Field</param>
    public async void Update(Field field)
    {
        if (field == null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        if (!field.IsBinaries && _item != null)
        {
            await Shell.Current.Navigation.PushAsync(new FieldEditPage(async (string k, string v, bool isProtected) => {
                _item.UpdateField(k, v, field.IsProtected);
                await dataStore.UpdateItemAsync(_item);
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
            throw new ArgumentNullException(nameof(field));
        }

        if (Fields.Remove(field) && _item != null)
        {
            _item.DeleteField(field);
            await dataStore.UpdateItemAsync(_item);
        }
        else
        {
            throw new NullReferenceException("Item is null or field cannot be found");
        }
    }

    [RelayCommand]
    private async void AddField(object obj)
    {
        if (_item == null) { throw new NullReferenceException("_item is null"); }

        await Shell.Current.Navigation.PushAsync(new FieldEditPage(async (string k, string v, bool isProtected) => {
            Field field = _item.AddField(k, v, isProtected);
            if (field != null && _item != null)
            {
                Fields.Add(field);
                await dataStore.UpdateItemAsync(_item);
            }
        }, _item));
    }

    [RelayCommand]
    private async void AddBinary(object obj)
    {
        List<string> inputTypeList = new List<string>()
            {
                Properties.Resources.field_id_file,
                Properties.Resources.field_id_camera,
                Properties.Resources.field_id_gallery
            };


        var typeValue = await Shell.Current.DisplayActionSheet(
            Properties.Resources.message_id_attachment_options, 
            Properties.Resources.action_id_cancel, null, inputTypeList.ToArray());
        
        try 
        {
            if (typeValue == Properties.Resources.field_id_gallery)
            {
                var photo = await MediaPicker.PickPhotoAsync();
                await LoadPhotoAsync(photo);
            }
            else if (typeValue == Properties.Resources.field_id_camera)
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                await LoadPhotoAsync(photo);
            }
            else if (typeValue == Properties.Resources.field_id_file)
            {
                await LoadFileAsync();
            }
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            // Feature is not supported on the device
            logger.LogError("{fnsEx.Message}", fnsEx.Message);
        }
        catch (PermissionException pEx)
        {
            // Permissions not granted
            logger.LogError("{pEx.Message}", pEx.Message);
        }
        catch (Exception ex)
        {
            logger.LogError("{ex.Message}", ex.Message);
        }
    }

    private async void AddBinaryField(string tempFilePath, string fileName)
    {
        if (_item == null) { throw new NullReferenceException("_item is null"); }

        var vBytes = File.ReadAllBytes(tempFilePath);
        Field field = _item.AddBinaryField(fileName, vBytes, Properties.Resources.label_id_attachment);
        if (field != null)
        {
            Fields.Add(field);
            await dataStore.UpdateItemAsync(_item);
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
        AddBinaryField(newFile, photo.FileName);
    }

    private async Task LoadFileAsync()
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
            AddBinaryField(tempFilePath, result.FileName);
            File.Delete(tempFilePath);
        }
        else
        {
            await Shell.Current.DisplayAlert(
                Properties.Resources.message_id_attachment_options,
                Properties.Resources.import_error_msg,
                Properties.Resources.alert_id_ok);
        }
    }
}
