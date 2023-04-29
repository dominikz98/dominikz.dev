using dominikz.Client.Api;
using dominikz.Client.Components.Instruments;
using dominikz.Domain.ViewModels.Songs;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Songs;

public partial class Song
{
    [Parameter] public Guid? SongId { get; set; }
    [Inject] protected SongsEndpoints? Endpoints { get; set; }
    private SongVm? _song;
    private Piano? _piano;

    protected override async Task OnInitializedAsync()
    {
        if (SongId == null)
            return;

        _song = await Endpoints!.GetById(SongId.Value);
    }
}