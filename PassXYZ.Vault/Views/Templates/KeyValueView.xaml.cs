using PassXYZ.Vault.Resources.Styles;
using PassXYZ.Vault.ViewModels;

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
                if(control == null) { throw new NullReferenceException(nameof(control)); }
                if(changingTo == null) { throw new NullReferenceException(nameof(changingTo)); }
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
                if (control == null) { throw new NullReferenceException(nameof(control)); }
                if (changingTo == null) { throw new NullReferenceException(nameof(changingTo)); }
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

    public static readonly BindableProperty SourceProperty =
        BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(KeyValueView), default!,
            propertyChanging: (bindable, oldValue, newValue) =>
            {
                var control = bindable as KeyValueView;
                var changingFrom = oldValue as ImageSource;
                var changingTo = newValue as ImageSource;
                if (control == null) { throw new NullReferenceException(nameof(control)); }
                if (changingTo == null) { throw new NullReferenceException(nameof(changingTo)); }
                control.Source = changingTo;
            });
    public ImageSource Source 
    {
        get { return (ImageSource)GetValue(SourceProperty); }
        set
        {
            imageField.Source = value;
            SetValue(SourceProperty, value);
        }
    }

    public static readonly BindableProperty ParentPageProperty =
        BindableProperty.Create(nameof(ParentPage), typeof(ContentPage), typeof(KeyValueView), default!,
            propertyChanging: (bindable, oldValue, newValue) =>
            {
                var control = bindable as KeyValueView;
            });
    public ContentPage ParentPage
    {
        get { return (ContentPage)GetValue(ParentPageProperty); }
        set
        {
            SetValue(ParentPageProperty, value);
        }
    }

    protected void SetContextAction(MenuItem contextAction, EventHandler handler, string? path = default)
    {
        contextAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
        if(path == null) 
        {
            if(handler == null) { throw new ArgumentNullException(nameof(handler)); }
            contextAction.Clicked += handler;
        }
        else 
        {
            contextAction.SetBinding(MenuItem.CommandProperty, new Binding { Source = BindingContext, Path = path });
        }
        ContextActions.Add(contextAction);
    }

    protected static MenuItem GetEditMenu()
    {
        return new MenuItem
        {
            Text = Properties.Resources.action_id_edit,
            IconImageSource = new FontImageSource
            {
                Glyph = FontAwesomeRegular.Edit,
                FontFamily = "FontAwesomeRegular",
                Color = Colors.Black,
                Size = 32
            }
        };
    }

    protected static MenuItem GetDeleteMenu()
    {
        return new MenuItem
        {
            Text = Properties.Resources.action_id_delete,
            IconImageSource = new FontImageSource
            {
                Glyph = FontAwesomeRegular.TrashAlt,
                FontFamily = "FontAwesomeRegular",
                Color = Colors.Black,
                Size = 32
            }
        };
    }

    protected static MenuItem GetCopyMenu()
    {
        return new MenuItem
        {
            Text = Properties.Resources.action_id_copy,
            IconImageSource = new FontImageSource
            {
                Glyph = FontAwesomeRegular.Copy,
                FontFamily = "FontAwesomeRegular",
                Color = Colors.Black,
                Size = 32
            }
        };
    }

    protected static MenuItem GetShowMenu()
    {
        return new MenuItem
        {
            Text = Properties.Resources.action_id_show,
            IconImageSource = new FontImageSource
            {
                Glyph = FontAwesomeRegular.Eye,
                FontFamily = "FontAwesomeRegular",
                Color = Colors.Black,
                Size = 32
            }
        };
    }
}