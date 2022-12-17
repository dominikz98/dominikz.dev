using dominikz.api.Provider;

namespace dominikz.api.Models.ViewModels;

public class NoobitArticleVm
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public NoobitTopicVm Topic { get; set; } = new();
    public string SeoTitle { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

public class NoobitTopicVm
{
    public string Name { get; set; } = string.Empty;
    public string SeoName { get; set; } = string.Empty;
    public NoobitBlogVm Blog { get; set; } = new();
}

public class NoobitBlogVm
{
    public string SeoName { get; set; } = string.Empty;
}