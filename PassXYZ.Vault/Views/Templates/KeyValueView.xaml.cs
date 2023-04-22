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

    public static readonly BindableProperty KeyProperty =
        BindableProperty.Create(nameof(Key), typeof(string), typeof(KeyValueView), string.Empty,
            propertyChanging: (bindable, oldValue, newValue) =>
            {
                var control = bindable as KeyValueView;
                var changingFrom = oldValue as string;
                var changingTo = newValue as string;
                control.Key = changingTo;
            });

    public string Key
    {
        get { return (string)GetValue(KeyProperty); }
        set
        {
            keyField.Text = value;
            SetValue(KeyProperty, value);
        }
    }

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(string), typeof(KeyValueView), string.Empty,
            propertyChanging: (bindable, oldValue, newValue) =>
            {
                var control = bindable as KeyValueView;
                var changingFrom = oldValue as string;
                var changingTo = newValue as string;
                control.Value = changingTo;
            });

    public string Value
    {
        get { return (string)GetValue(ValueProperty); }
        set
        {
            valueField.Text = value;
            SetValue(ValueProperty, value);
        }
    }

}