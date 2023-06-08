using KPCLib;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using PassXYZLib;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Channels;

namespace PassXYZ.BlazorUI;

public partial class EditFormDialog<TItem>
{
    [Parameter]
    public string Id { get; set; } = default!;
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;
    [Parameter]
    public Action? OnClose { get; set; }
    [Parameter]
    public Func<string, string, Task<bool>>? OnSaveAsync { get; set; }
    [Parameter]
    [NotNull]
    public string CloseButtonText { get; set; } = "Cancel";
    [Parameter]
    [NotNull]
    public string SaveButtonText { get; set; } = "Save";

    [Parameter]
    public KeyValueData<TItem> ModelData { get; set; }

    [Parameter]
    public string? KeyPlaceHolder { get; set; }
    [Parameter]
    public string? ValuePlaceHolder { get; set; }

#pragma warning disable BL0007
    bool _isKeyEditingEnable = false;
    [Parameter]
    public bool IsKeyEditingEnable
    {
        get => _isKeyEditingEnable;
        set
        {
            if (value != _isKeyEditingEnable)
            {
                _isKeyEditingEnable = value;
                IsKeyEditingEnableChanged?.InvokeAsync(_isKeyEditingEnable);
                Debug.WriteLine($"EditFormDialog: _isKeyEditingEnable={_isKeyEditingEnable}");
            }
        }
    }
#pragma warning restore BL0007
    [Parameter]
    public EventCallback<bool>? IsKeyEditingEnableChanged { get; set; }

    private string dataDismiss = string.Empty;

    public EditFormDialog()
    {
        ModelData = new();
    }

    private void OnClickClose()
    {
        Debug.WriteLine($"EditFormDialog.OnClickClose: DialogId={Id}");
        OnClose?.Invoke();
    }

    private void OnClickSave()
    {
        Debug.WriteLine($"EditFormDialog.OnClickSave: DialogId={Id}");
        SetSaveButtonText();
    }

    private async Task HandleValidSubmit()
    {
        if (OnSaveAsync != null && ModelData.IsChanged)
        {
            await OnSaveAsync(ModelData.Key, ModelData.Value);
            ModelData.IsChanged = false;
            Debug.WriteLine($"EditFormDialog.HandleValidSubmit: {ModelData.Key} {ModelData.Value}");
        }
    }

    private void KeyHandler() 
    {
        SetSaveButtonText(true);
    }

    private void SetSaveButtonText(bool changed = false)
    {
        if (ModelData == null) return;
        if (!ModelData.IsValid || changed)
        {
            dataDismiss = string.Empty;
            SaveButtonText = "Save";
        }
        else
        {
            dataDismiss = "modal";
            SaveButtonText = "Close";
        }
    }
}