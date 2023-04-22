using System.Diagnostics;
using KPCLib;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views;

public partial class ItemsPage : ContentPage
{
    ItemsViewModel viewModel;

    public ItemsPage(ItemsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = this.viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.OnAppearing();
    }
}