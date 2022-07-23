using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PassXYZ.BlazorUI;

public partial class ModalDialog : IDisposable
{
    [Parameter]
    public string? Id { get; set; }
    [Parameter]
    public string? Title { get; set; }
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;
    [Parameter]
    public Func<Task>? OnClose { get; set; }
    [Parameter]
    public Func<Task<bool>>? OnSaveAsync { get; set; }
    [Parameter]
    [NotNull]
    public string? CloseButtonText { get; set; }
    [Parameter]
    [NotNull]
    public string? SaveButtonText { get; set; }

    private async Task OnClickClose()
    {
        if (OnClose != null)
        {
            await OnClose();
        }
    }

    private async Task OnClickSave() 
    {
        if (OnSaveAsync != null)
        {
            await OnSaveAsync();
        }
    }

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
        Debug.WriteLine($"ModalDialog: delete - {Id}");
    }
}