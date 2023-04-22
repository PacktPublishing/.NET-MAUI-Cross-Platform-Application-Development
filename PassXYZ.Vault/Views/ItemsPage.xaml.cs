using System.Diagnostics;
using PassXYZ.Vault.Models;
using PassXYZ.Vault.ViewModels;
using PassXYZ.Vault.Views;

namespace PassXYZ.Vault.Views
{

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
}