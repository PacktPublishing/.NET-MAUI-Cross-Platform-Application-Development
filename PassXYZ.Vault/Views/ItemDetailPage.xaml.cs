using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;
using HybridWebView;
using KPCLib;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views;

public partial class ItemDetailPage : ContentPage
{
    public ItemDetailPage(ItemDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}