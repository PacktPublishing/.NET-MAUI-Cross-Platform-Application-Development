using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views
{
    public partial class NewItemPage : ContentPage
    {
        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}