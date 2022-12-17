using dominikz.dev.Endpoints;
using dominikz.shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages.Media;

public partial class Movie
{
    [Parameter]
    public Guid MovieId { get; set; }

    [Inject]
    protected MovieEndpoints? Endpoints { get; set; }

    private MovieDetailVM? _movie;

    protected override async Task OnInitializedAsync()
        => _movie = await Endpoints!.GetById(MovieId);
}
