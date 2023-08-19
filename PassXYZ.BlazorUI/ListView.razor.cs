using Microsoft.AspNetCore.Components;

namespace PassXYZ.BlazorUI;

public partial class ListView<TItem> 
{
    [Parameter]
    public RenderFragment? Header { get; set; }

    [Parameter]
    public RenderFragment<TItem>? Row { get; set; }

    [Parameter]
    public IEnumerable<TItem>? Items { get; set; }

    [Parameter]
    public RenderFragment? Footer { get; set; }
}