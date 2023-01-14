namespace dominikz.Domain.ViewModels.Blog;

public class ArticleViewVm : ArticleVm
{
    public string Text { get; init; } = string.Empty;
    public List<string> Tags { get; init; } = new();
}