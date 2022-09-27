namespace PassXYZ.Vault.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagePreviewPage : ContentPage
    {
        public ImagePreviewPage(byte[] binary) : this()
        {
            if(binary != null)
            {
                imageView.Source = ImageSource.FromStream(() => new MemoryStream(binary));
            }
        }

        public ImagePreviewPage ()
        {
            InitializeComponent();
        }

        private async void OnCloseClickedAsync(object sender, EventArgs e)
        {
            _ = await Navigation.PopAsync();
        }
    }
}