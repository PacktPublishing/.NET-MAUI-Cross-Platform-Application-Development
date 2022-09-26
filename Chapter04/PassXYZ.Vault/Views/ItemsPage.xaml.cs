using System.Diagnostics;
using KPCLib;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views;

public partial class ItemsPage : ContentPage
{
    ItemsViewModel _viewModel;

    public ItemsPage()
    {
        InitializeComponent();

        BindingContext = _viewModel = new ItemsViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }

    void OnItemSelected(object sender, SelectedItemChangedEventArgs args) 
    {
        var item = args.SelectedItem as Item;
        if (item == null)
        {
            Debug.WriteLine("OnItemSelected: item is null.");
            return;
        }
        _viewModel.OnItemSelected(item);
    }
}