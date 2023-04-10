using dominikz.Client.Components.Toast;
using dominikz.Domain.Enums;
using dominikz.Domain.ViewModels.Media;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Media;

public partial class Movie
{
    [Parameter] public Guid MovieId { get; set; }
    [Inject] protected MovieEndpoints? Endpoints { get; set; }
    [Inject] protected ICredentialStorage? Credentials { get; set; }
    [Inject] protected NavigationManager? NavManager { get; set; }
    [Inject] protected ToastService? Toast { get; set; }
    [Inject] protected ApiClient? ApiClient { get; set; }

    private MovieDetailVm? _movie;
    private bool _hasCreatePermission;
    private bool _hasStreamPermission;
    private string _streamUrl = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _hasCreatePermission = await Credentials!.HasRight(PermissionFlags.CreateOrUpdate | PermissionFlags.Media);
        _hasStreamPermission = await Credentials!.HasRight(PermissionFlags.Media);
        _movie = await Endpoints!.GetById(MovieId);

        if (_hasCreatePermission == false || _movie == null)
            return;

        var stream = await Endpoints!.CreateStreamingToken(MovieId);
        if (string.IsNullOrWhiteSpace(stream?.Token))
            return;

        _streamUrl = Endpoints!.CreateStreamingUrl(MovieId, stream.Token);
    }
}