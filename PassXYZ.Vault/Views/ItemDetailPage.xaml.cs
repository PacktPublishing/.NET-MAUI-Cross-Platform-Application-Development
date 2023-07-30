using System.Diagnostics;
using KPCLib;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views;

public partial class ItemDetailPage : ContentPage
{
    ItemDetailViewModel _viewModel;

    public ItemDetailPage(ItemDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        fieldsListView.IsVisible = !_viewModel.IsNotes;
        markdownview.IsVisible = _viewModel.IsNotes;
        if (_viewModel.IsNotes)
        {
            htmlSource.Html = _viewModel.Description;
        }
    }

    protected override void OnDisappearing() 
    { 
        base.OnDisappearing();
        if (_viewModel.IsNotes) 
        {
            htmlSource.Html = "<p>Loading ...</p>";
        }
    }
}