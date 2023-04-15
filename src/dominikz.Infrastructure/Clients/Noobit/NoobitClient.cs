using System.Net.Http.Json;
using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Models;

namespace dominikz.Infrastructure.Clients.Noobit;

public class NoobitClient
{
    private readonly HttpClient _client;
    private const string Api = "api";

    public NoobitClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyCollection<ExtArticleShadow>> GetArticles(CancellationToken cancellationToken)
    {
        var codingArticles = await GetArticleByCategory(ArticleCategoryEnum.Coding, cancellationToken);
        var travelArticles = await GetArticleByCategory(ArticleCategoryEnum.Travel, cancellationToken);
        var birdsArticles = await GetArticleByCategory(ArticleCategoryEnum.Birds, cancellationToken);
        var thoughtsArticles = await GetArticleByCategory(ArticleCategoryEnum.Thoughts, cancellationToken);

        return codingArticles.Union(travelArticles)
            .Union(birdsArticles)
            .Union(thoughtsArticles)
            .ToList();
    }

    private async Task<IReadOnlyCollection<ExtArticleShadow>> GetArticleByCategory(ArticleCategoryEnum category, CancellationToken cancellationToken)
    {
        var vms = await _client.GetFromJsonAsync<List<NoobitArticleVm>>($"{Api}/blog/{category}/overview", cancellationToken) ?? new List<NoobitArticleVm>();

        // attach article url
        var shadows = new List<ExtArticleShadow>();
        foreach (var vm in vms)
        {
            var shadow = new ExtArticleShadow()
            {
                Title = vm.Title,
                Date = DateOnly.FromDateTime(vm.Date),
                Category = Enum.Parse<ArticleCategoryEnum>(vm.Topic.Blog.SeoName, true),
                Url = $"{_client.BaseAddress}blog/{category}/{vm.Topic.SeoName}/{vm.SeoTitle}".ToLower(),
                Source = ArticleSourceEnum.Noobit
            };
            await AssignImage(shadow, $"{_client.BaseAddress}assets/blog/{category}/index.webp".ToLower(), cancellationToken);
            shadows.Add(shadow);
        }

        return shadows;
    }
    
    private static async Task AssignImage(ExtArticleShadow shadow, string imageUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            shadow.ImageId = Guid.Empty;
            return;
        }

        try
        {
            shadow.Image = await new HttpClient().GetStreamAsync(imageUrl, cancellationToken);
            shadow.ImageId = Guid.NewGuid();
        }
        catch (Exception)
        {
            shadow.ImageId = Guid.Empty;
        }
    }
}