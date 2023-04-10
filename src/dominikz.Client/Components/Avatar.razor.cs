using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components;

public partial class Avatar
{
    [Parameter] public string Name { get; set; } = string.Empty;

    [Parameter] public string ImageSource { get; set; } = string.Empty;
}