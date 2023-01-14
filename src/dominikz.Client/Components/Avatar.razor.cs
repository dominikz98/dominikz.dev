using dominikz.Domain.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components;

public partial class Avatar
{
    [Parameter]
    public PersonVm? Person { get; set; }
}
