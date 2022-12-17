using dominikz.shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components;

public partial class Avatar
{
    [Parameter]
    public PersonVM? Person { get; set; }
}
