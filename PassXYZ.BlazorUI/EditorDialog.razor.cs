using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PassXYZ.BlazorUI;

public partial class EditorDialog
{
    bool _isKeyEditingEnable = false;
    [Parameter]
    public bool IsKeyEditingEnable 
    {
        get => _isKeyEditingEnable;
        set 
        {
            if(value != _isKeyEditingEnable) 
            {
                _isKeyEditingEnable = value;
                IsKeyEditingEnableChanged?.InvokeAsync(_isKeyEditingEnable);
                Debug.WriteLine($"EditorDialog: _isKeyEditingEnable={_isKeyEditingEnable}");
            }
        }
    }
    [Parameter]
    public EventCallback<bool>? IsKeyEditingEnableChanged { get; set; }

    string _key = string.Empty;
    [Parameter]
    public string Key
    {
        get => _key;
        set
        {
            if(_key != value) 
            {
                _key = value;
                KeyChanged?.InvokeAsync(_key);
                Debug.WriteLine($"EditorDialog: Key={_key}");
            }
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
            if(value != _value) 
            {
                _value = value ?? string.Empty;
                ValueChanged?.InvokeAsync(_value);
                Debug.WriteLine($"EditorDialog: Value={_value}");
            }
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

    [Parameter]
    public Action? OnClose { get; set; }

    async Task<bool> OnSaveClicked()
    {
        OnSave?.Invoke(Key, Value);
        Debug.WriteLine($"EditorDialog: OnSaveClicked(key={Key}, value={Value})");
        return true;
    }

    void OnCloseClicked() 
    {
        OnClose?.Invoke();
    }
}