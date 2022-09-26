using KPCLib;
using PassXYZLib;

namespace PassXYZ.Vault.ViewModels;

[QueryProperty(nameof(Type), nameof(Type))]
public class NewItemViewModel : BaseViewModel
{
    private string text;
    private string description;
    private ItemSubType _type = ItemSubType.Group;

    public NewItemViewModel()
    {
        SaveCommand = new Command(OnSave, ValidateSave);
        CancelCommand = new Command(OnCancel);
        this.PropertyChanged +=
            (_, __) => SaveCommand.ChangeCanExecute();
        Title = "New Item";
    }

    private void SetTitle(ItemSubType type)
    {
        if (type == ItemSubType.Group)
        {
            Title = Properties.Resources.action_id_add + " " + Properties.Resources.item_subtype_group;
        }
        else
        {
            Title = Properties.Resources.action_id_add + " " + Properties.Resources.item_subtype_entry;
        }
    }

    private bool ValidateSave()
    {
        return !String.IsNullOrWhiteSpace(text)
            && !String.IsNullOrWhiteSpace(description);
    }

    public ItemSubType Type
    {
        get => _type;
        set
        {
            _ = SetProperty(ref _type, value);
            SetTitle(_type);
        }
    }

    public string Text
    {
        get => text;
        set => SetProperty(ref text, value);
    }

    public string Description
    {
        get => description;
        set => SetProperty(ref description, value);
    }

    public Command SaveCommand { get; }
    public Command CancelCommand { get; }

    private async void OnCancel()
    {
        // This will pop the current page off the navigation stack
        await Shell.Current.GoToAsync("..");
    }

    private async void OnSave()
    {
        Item? newItem = DataStore.CreateNewItem(_type);

        if (newItem != null)
        {
            newItem.Name = Text;
            newItem.Notes = Description;
            await DataStore.AddItemAsync(newItem);
        }

        // This will pop the current page off the navigation stack
        await Shell.Current.GoToAsync("..");
    }
}
