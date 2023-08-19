using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;

namespace PassXYZ.BlazorUI;

public partial class Navbar 
{
    [Parameter]
    public string? ParentLink { get; set; }

    [Parameter]
    public string? DialogId { get; set; }

    [Parameter]
    public string? Title { get; set; }
    [Parameter]
    public EventCallback<MouseEventArgs> OnAddClick { get; set; }
    private void OnClickClose(MouseEventArgs e)
    {
        OnAddClick.InvokeAsync();
        //StateHasChanged();
    }
}