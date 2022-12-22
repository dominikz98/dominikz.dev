using dominikz.dev.Components.Instruments;
using dominikz.dev.Endpoints;
using dominikz.shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages.Music;

public partial class Song
{

    [Inject]
    protected MusicEndpoints? Endpoints { get; set; }

    private SongVm? _song;
    private Piano? _piano;

    protected override async Task OnInitializedAsync()
        => _song = await Endpoints!.GetSongById(Guid.Parse("60a00835-57b3-11ed-8c93-00d861ff2f96"));

    private void CallOnPlayClicked()
        => _piano?.Play();
}
