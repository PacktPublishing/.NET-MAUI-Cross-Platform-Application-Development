using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;
using HybridWebView;
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
#if DEBUG
        markdownview.EnableWebDevTools = true;
#endif
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        fieldsListView.IsVisible = !_viewModel.IsNotes;
        markdownview.IsVisible = _viewModel.IsNotes;
        if(_viewModel.IsNotes) 
        {
            markdownview.Reload();
        }
    }

    protected override void OnDisappearing() 
    { 
        base.OnDisappearing();
    }

    private async void OnHybridWebViewRawMessageReceived(object sender, HybridWebView.HybridWebViewRawMessageReceivedEventArgs e)
    {
        string markDownTxt = HttpUtility.JavaScriptStringEncode(_viewModel.MarkdownText);
        Debug.WriteLine($"markDownTxt len={markDownTxt.Length}");
        await markdownview.InvokeJsMethodAsync("MarkdownToHtml", markDownTxt);
    }
}