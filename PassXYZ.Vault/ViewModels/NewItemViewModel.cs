using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Services;
using static System.Net.Mime.MediaTypeNames;

namespace PassXYZ.Vault.ViewModels
{
    [QueryProperty(nameof(Type), nameof(Type))]
    public partial class NewItemViewModel : ObservableObject
    {
        readonly IDataStore<Item>? dataStore;
        ILogger<NewItemViewModel> logger;
        private ItemSubType _type = ItemSubType.Group;

        public NewItemViewModel(IDataStore<Item> dataStore, ILogger<NewItemViewModel> logger)
        {
            if (dataStore == null) { throw new ArgumentNullException(nameof(dataStore)); }
            this.dataStore = dataStore;
            this.logger = logger;
        }

        private void SetPlaceholder(ItemSubType type)
        {
            if (type == ItemSubType.Group)
            {
                Placeholder = Properties.Resources.action_id_add + " " + Properties.Resources.item_subtype_group;
            }
            else
            {
                Placeholder = Properties.Resources.action_id_add + " " + Properties.Resources.item_subtype_entry;
            }
        }

        public ItemSubType Type
        {
            get => _type;
            set
            {
                _ = SetProperty(ref _type, value);
                SetPlaceholder(_type);
            }
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string? name;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string? description;

        [ObservableProperty]
        private string? placeholder;

        [RelayCommand]
        private async void Cancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand(CanExecute = nameof(ValidateSave))]
        private async void Save()
        {
            if(dataStore == null) { throw new ArgumentNullException("dataStore cannot be null"); }
            Item? newItem = dataStore.CreateNewItem(_type);

            if (newItem != null)
            {
                newItem.Name = Name;
                newItem.Notes = Description;
                await dataStore.AddItemAsync(newItem);
            }
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
