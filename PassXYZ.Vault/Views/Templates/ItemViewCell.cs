using System.Diagnostics;

using KPCLib;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views.Templates
{
    public class ItemViewCell : KeyValueView
    {
        public ItemViewCell()
        {
            SetContextAction(GetEditMenu(), OnEditAction);
            SetContextAction(GetDeleteMenu(), OnDeleteAction);
        }

        private void OnEditAction(object? sender, System.EventArgs e)
        {
            if(sender is MenuItem menuItem)
            {
                if(menuItem.CommandParameter is Item item && 
                    ParentPage.BindingContext is ItemsViewModel vm)
                {
                    Debug.WriteLine($"ItemViewCell: edit action on {item.Name}");
                    vm.Update(item);
                }
            }
        }

        private void OnDeleteAction(object? sender, System.EventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (menuItem.CommandParameter is Item item &&
                    ParentPage.BindingContext is ItemsViewModel vm)
                {
                    Debug.WriteLine($"ItemViewCell: delete action on {item.Name}");
                    vm.Delete(item);
                }
            }
        }
    }
}
