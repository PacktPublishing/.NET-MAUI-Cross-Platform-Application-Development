﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PassXYZ.Vault.Models;
using PassXYZ.Vault.Services;

namespace PassXYZ.Vault.ViewModels
{
    public partial class NewItemViewModel : ObservableObject
    {
        readonly IDataStore<Item>? _dataStore;
        readonly ILogger<NewItemViewModel> _logger;

        public NewItemViewModel(IDataStore<Item> dataStore, ILogger<NewItemViewModel> logger)
        {
            this._dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            this._logger = logger;
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
            if (_dataStore == null) { throw new NullReferenceException("_dataStore is null"); }

            _logger.LogDebug("Save: Name: {name}", Name);
            Item newItem = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Name,
                Description = Description
            };

            _ = await _dataStore.AddItemAsync(newItem);
            Name = string.Empty;
            Description = string.Empty;

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private bool ValidateSave()
        {
            var canExecute = !String.IsNullOrWhiteSpace(Name)
                && !String.IsNullOrWhiteSpace(Description);
            _logger.LogDebug("ValidateSave: {canExecute}", canExecute);
            return canExecute;
        }
    }
}
