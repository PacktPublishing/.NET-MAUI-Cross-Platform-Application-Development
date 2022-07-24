using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PassXYZ.BlazorUI;

public partial class EditorDialog
{
    [Parameter]
    public string? Id { get; set; }
    [Parameter]
    public bool IsKeyEditingEnable { get; set; }

    string _key = string.Empty;
    [Parameter]
    public string Key
    {
        get => _key;
        set
        {
            _key = value;
            KeyChanged?.InvokeAsync(_key);
            Debug.WriteLine($"EditorDialog: Key={_key}");
        }
    }
    [Parameter]
    public EventCallback<string>? KeyChanged { get; set; }
    [Parameter]
    public string? KeyPlaceHolder { get; set; }

    string _value = string.Empty;
    [Parameter]
    public string Value 
    {
        get => _value;
        set 
        {
            _value = value ?? string.Empty;
            ValueChanged?.InvokeAsync(_value);
            Debug.WriteLine($"EditorDialog: Value={_value}");
        }
    }
    [Parameter]
    public EventCallback<string>? ValueChanged { get; set; }
    [Parameter]
    public string? ValuePlaceHolder { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public Action<string, string>? OnSave { get; set; }

    async Task<bool> OnSaveClicked()
    {
        //TestField.Key = key;
        //TestField.Value = value;
        OnSave?.Invoke(Key, Value);
        Debug.WriteLine($"EditorDialog: OnSaveClicked(key={Key}, value={Value})");
        return true;
    }
}