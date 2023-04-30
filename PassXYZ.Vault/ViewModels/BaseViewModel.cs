using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassXYZ.Vault.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        [RelayCommand]
        private void ItemSelectionChanged(object sender)
        {
            OnSelection(sender);
        }

        public abstract void OnSelection(object sender);
    }
}
