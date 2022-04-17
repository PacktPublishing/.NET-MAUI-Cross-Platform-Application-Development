using KPCLib;
using PassXYZLib;

namespace PassXYZ.Vault.ViewModels;

public class NewItemViewModel : BaseViewModel
{
    private string text;
    private string description;

    public NewItemViewModel()
    {
        SaveCommand = new Command(OnSave, ValidateSave);
        CancelCommand = new Command(OnCancel);
        this.PropertyChanged +=
            (_, __) => SaveCommand.ChangeCanExecute();
    }

    private bool ValidateSave()
    {
        return !String.IsNullOrWhiteSpace(text)
            && !String.IsNullOrWhiteSpace(description);
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
        // TODO: need to check item type and create PwGroup or PwEntry
        Item newItem = new PxEntry()
        {
            Name = Text,
            Notes = Description
        };

        await DataStore.AddItemAsync(newItem);

        // This will pop the current page off the navigation stack
        await Shell.Current.GoToAsync("..");
    }
}
