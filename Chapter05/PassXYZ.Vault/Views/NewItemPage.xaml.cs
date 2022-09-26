using KPCLib;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}