using PassXYZ.Vault.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace PassXYZ.Vault.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}