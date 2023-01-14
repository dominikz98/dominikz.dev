using dominikz.Domain.Enums;
using dominikz.Domain.ViewModels.Media;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Media;

public partial class Movie
{
    [Parameter] public Guid MovieId { get; set; }
    [Inject] protected MovieEndpoints? Endpoints { get; set; }
    [Inject] protected  ICredentialStorage? Credentials { get; set; }
    [Inject] protected  NavigationManager? NavManager { get; set; }
    
    private MovieDetailVm? _movie;
    private bool _hasCreatePermission;

    protected override async Task OnInitializedAsync()
    {
        _hasCreatePermission = await Credentials!.HasRight(PermissionFlags.CreateOrUpdate | PermissionFlags.Blog);
        _movie = await Endpoints!.GetById(MovieId);
    } 
}