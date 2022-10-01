using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Pages;

public partial class BlogDetail
{
    [Parameter]
    public Guid? ArticleId { get; set; }
}
