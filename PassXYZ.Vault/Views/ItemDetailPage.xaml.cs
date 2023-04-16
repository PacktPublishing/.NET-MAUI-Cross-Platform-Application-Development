using System.Diagnostics;
using KPCLib;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}