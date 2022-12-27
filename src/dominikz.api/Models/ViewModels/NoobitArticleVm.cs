using System.Diagnostics.CodeAnalysis;

namespace dominikz.api.Models.ViewModels;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class NoobitArticleVm
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
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