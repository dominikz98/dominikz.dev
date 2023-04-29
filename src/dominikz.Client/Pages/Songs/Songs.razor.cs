using dominikz.Client.Api;
using dominikz.Client.Components;
using dominikz.Client.Extensions;
using dominikz.Client.Tables;
using dominikz.Domain.Filter;
using dominikz.Domain.ViewModels.Songs;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Songs;

public partial class Songs
{
    [Inject] internal SongsEndpoints? Endpoints { get; set; }
    [Inject] protected NavigationManager? NavManager { get; set; }

    private TextBox? _searchbox;
    private List<SongVm> _songs = new();
    private bool _isTableView;

    protected override async Task OnInitializedAsync()
    {
        var filter = CreateFilter();
        _searchbox?.SetValue(filter.Text);
        
        var init = NavManager!.TrackQuery(SearchSongs);
        if (init)
            await SearchSongs();
    }

    private async Task SearchSongs()
    {
        var filter = CreateFilter();
        _songs = (await Endpoints!.Search(filter)).ToList();
        StateHasChanged();
    }

    private SongsFilter CreateFilter()
        => new()
        {
            Text = NavManager!.GetQueryParamByKey(QueryNames.Blog.Search)
        };
}