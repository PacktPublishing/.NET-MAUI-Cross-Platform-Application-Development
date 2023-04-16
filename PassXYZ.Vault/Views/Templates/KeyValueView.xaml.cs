namespace PassXYZ.Vault.Views.Templates;

/// <summary>
/// This is a view to display a pair of key value with an icon.
/// +------+--------------+
/// |      | Name         |
/// | Icon | -------------+
/// |      | Description  |
/// +------+--------------+
/// This view is a 2x2 grid. The first column is used to display an icon.
/// The second column is used to display a key value pair.
/// </summary>
public partial class KeyValueView : ViewCell
{
	public KeyValueView()
	{
		InitializeComponent();
	}
}