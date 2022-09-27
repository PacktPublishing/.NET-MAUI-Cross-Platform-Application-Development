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
        Debug.WriteLine($"ItemsPage {_viewModel.Title} created");
    }

    ~ItemsPage() 
    {
        Debug.WriteLine($"~ItemsPage {_viewModel.Title} destroyed");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }

    private void OnMenuEdit(object sender, EventArgs e)
    {
        var mi = (MenuItem)sender;

        if (mi.CommandParameter is Item item)
        {
            _viewModel.Update(item);
        }
    }

    private async void OnMenuDeleteAsync(object sender, EventArgs e)
    {
        var mi = (MenuItem)sender;

        if (mi.CommandParameter is Item item)
        {
            await _viewModel.DeletedAsync(item);
            Debug.WriteLine("ItemsPage: OnMenuDeleteAsync clicked");
        }
    }

    void OnItemSelected(object sender, SelectedItemChangedEventArgs args) 
    {
        var item = args.SelectedItem as Item;
        if (item == null)
        {
            Debug.WriteLine("ItemsPage: OnItemSelected - item is null.");
            return;
        }
        Debug.WriteLine($"ItemsPage: SelectedItem is {item.Name}");
        _viewModel.OnItemSelected(item);
    }
}