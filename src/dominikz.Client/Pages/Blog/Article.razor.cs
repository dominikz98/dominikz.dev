using dominikz.Client.Api;
using dominikz.Domain.Enums;
using dominikz.Domain.ViewModels.Blog;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Blog;

public partial class Article
{
    [Parameter] public Guid? ArticleId { get; set; }
    [Inject] internal BlogEndpoints? Endpoints { get; set; }
    [Inject] protected NavigationManager? NavManager { get; set; }
    [Inject] protected ICredentialStorage? Credentials { get; set; }
    
    private ArticleViewVm? _article;
    private bool _hasCreatePermission;

    protected override async Task OnInitializedAsync()
    {
        if (ArticleId is null)
            return;

        _hasCreatePermission = await Credentials!.HasRight(PermissionFlags.CreateOrUpdate | PermissionFlags.Blog);
        _article = await Endpoints!.GetById(ArticleId.Value);
    }
}
