using System.Diagnostics;
using KPCLib;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views;

public partial class ItemDetailPage : ContentPage
{
    public ItemDetailPage()
    {
        InitializeComponent();
        BindingContext = new ItemDetailViewModel();
    }

    void OnFieldSelected(object sender, SelectedItemChangedEventArgs args)
    {
        var field = args.SelectedItem as Field;
        if (field == null)
        {
            Debug.WriteLine("OnFieldSelected: Field is null.");
            return;
        }
    }
}