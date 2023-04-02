using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassXYZ.Vault.Models;
using PassXYZ.Vault.Services;

namespace PassXYZ.Vault.ViewModels
{
    public partial class NewItemViewModel : ObservableObject
    {
        readonly IDataStore<Item>? dataStore;

        public NewItemViewModel(IDataStore<Item> dataStore)
        {
            if (dataStore == null) { throw new ArgumentNullException(nameof(dataStore)); }
            this.dataStore = dataStore;
        }

        [ObservableProperty]
        private string? name;

        [ObservableProperty]
        private string? description;

        [RelayCommand]
        private async void Cancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand(CanExecute = nameof(ValidateSave))]
        private async void Save()
        {
            Item newItem = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Name,
                Description = Description
            };

            _ = await dataStore.AddItemAsync(newItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(Name)
                && !String.IsNullOrWhiteSpace(Description);
        }
    }
}
