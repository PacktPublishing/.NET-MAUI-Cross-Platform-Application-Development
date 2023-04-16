using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Services;

namespace PassXYZ.Vault.ViewModels
{
    public partial class NewItemViewModel : ObservableObject
    {
        readonly IDataStore<Item>? dataStore;
        ILogger<NewItemViewModel> logger;

        public NewItemViewModel(IDataStore<Item> dataStore, ILogger<NewItemViewModel> logger)
        {
            if (dataStore == null) { throw new ArgumentNullException(nameof(dataStore)); }
            this.dataStore = dataStore;
            this.logger = logger;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string? name;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
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
            logger.LogDebug("Save: Name: {name}", Name);
            Item newItem = new PxEntry()
            {
                Name = Name,
                Notes = Description
            };

            _ = await dataStore.AddItemAsync(newItem);
            Name = string.Empty;
            Description = string.Empty;

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private bool ValidateSave()
        {
            var canExecute = !String.IsNullOrWhiteSpace(Name)
                && !String.IsNullOrWhiteSpace(Description);
            logger.LogDebug("ValidateSave: {canExecute}", canExecute);
            return canExecute;
        }
    }
}
