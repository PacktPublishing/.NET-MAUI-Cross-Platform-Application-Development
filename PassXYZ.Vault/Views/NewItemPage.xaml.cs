using KPCLib;
using PassXYZ.Vault.ViewModels;

namespace PassXYZ.Vault.Views
{
    public partial class NewItemPage : ContentPage
    {
        public NewItemPage(NewItemViewModel viewModel)
        {
            InitializeComponent();
            if(viewModel == null ) { throw new ArgumentNullException(nameof(viewModel)); }
            BindingContext = viewModel;
        }
    }
}