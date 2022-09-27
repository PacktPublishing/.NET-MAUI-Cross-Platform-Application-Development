using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace PassXYZ.BlazorUI;

public partial class Dropdown 
{
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;
}