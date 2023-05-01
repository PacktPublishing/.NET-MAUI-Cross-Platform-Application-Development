using System.Diagnostics;

using KPCLib;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views.Templates
{
    public class FieldViewCell : KeyValueView
    {
        public FieldViewCell() 
        {
            SetContextAction(GetEditMenu(), OnEditAction);
            SetContextAction(GetDeleteMenu(), OnDeleteAction);
            SetContextAction(GetCopyMenu(), OnCopyAction);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is Field field)
            {
                if (field.IsProtected)
                    SetContextAction(GetShowMenu(), OnShowAction);
            }
        }

        private void OnEditAction(object? sender, System.EventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (menuItem.CommandParameter is Field field &&
                    ParentPage.BindingContext is ItemDetailViewModel vm)
                {
                    Debug.WriteLine($"FieldViewCell: edit action on {field.Key}");
                    vm.Update(field);
                }
            }
        }

        private void OnDeleteAction(object? sender, System.EventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (menuItem.CommandParameter is Field field &&
                    ParentPage.BindingContext is ItemDetailViewModel vm)
                {
                    Debug.WriteLine($"FieldViewCell: delete action on {field.Key}");
                    vm.DeletedAsync(field);
                }
            }
        }

        private async void OnCopyAction(object? sender, System.EventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (menuItem.CommandParameter is Field field)
                {
                    if (field.IsProtected)
                    {
                        await Clipboard.SetTextAsync(field.EditValue);
                    }
                    else
                    {
                        await Clipboard.SetTextAsync(field.Value);
                    }
                }
            }
        }

        private void OnShowAction(object? sender, System.EventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (menuItem.CommandParameter is Field field)
                {
                    if (field.IsProtected)
                    {
                        if (field.IsHide)
                        {
                            field.ShowPassword();
                            Value = field.Value;
                            menuItem.Text = Properties.Resources.action_id_hide;
                        }
                        else
                        {
                            field.HidePassword();
                            Value = field.Value;
                            menuItem.Text = Properties.Resources.action_id_show;
                        }
                    }
                }
            }
        }
    }
}
