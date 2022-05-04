using System.Diagnostics;
using KPCLib;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views;

public partial class ItemDetailPage : ContentPage
{
    ItemDetailViewModel _viewModel;
    public ItemDetailPage()
    {
        InitializeComponent();
        BindingContext = _viewModel = new ItemDetailViewModel();
        Debug.WriteLine($"ItemDetailPage {_viewModel.Title} created");
    }

    ~ItemDetailPage() 
    {
        Debug.WriteLine($"~ItemDetailPage {_viewModel.Title} destroyed");
    }

    void OnFieldSelected(object sender, SelectedItemChangedEventArgs args)
    {
        var field = args.SelectedItem as Field;
        if (field == null)
        {
            Debug.WriteLine("ItemDetailPage: Field is null in OnFieldSelected.");
            return;
        }
    }
}