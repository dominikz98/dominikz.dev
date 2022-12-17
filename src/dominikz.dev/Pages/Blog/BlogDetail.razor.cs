using dominikz.dev.Endpoints;
using dominikz.shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages.Blog;

public partial class BlogDetail
{
    [Parameter]
    public Guid? ArticleId { get; set; }

    [Inject]
    protected BlogEndpoints? Endpoints { get; set; }

    private ArticleDetailVm? _article;

    protected override async Task OnInitializedAsync()
    {
        if (ArticleId is null)
            return;

        _article = await Endpoints!.GetById(ArticleId.Value);
    }
}
