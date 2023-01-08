using dominikz.dev.Endpoints;
using dominikz.dev.Utils;
using dominikz.shared.Enums;
using dominikz.shared.ViewModels.Media;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages.Media;

public partial class Movie
{
    [Parameter] public Guid MovieId { get; set; }
    [Inject] protected MovieEndpoints? Endpoints { get; set; }
    [Inject] protected  CredentialStorage? Credentials { get; set; }
    [Inject] protected  NavigationManager? NavManager { get; set; }
    
    private MovieDetailVm? _movie;
    private bool _hasCreatePermission;

    protected override async Task OnInitializedAsync()
    {
        _hasCreatePermission = await Credentials!.HasRight(PermissionFlags.CreateOrUpdate | PermissionFlags.Blog);
        _movie = await Endpoints!.GetById(MovieId);
    } 
}