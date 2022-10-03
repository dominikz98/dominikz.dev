using dominikz.kernel.ViewModels;
using Markdig;

namespace dominikz.kernel.Endpoints;

public class ArticlesEndpoints
{
    private readonly List<ArticleDetailVM> _articles = new()
        {
            new ArticleDetailVM()
            {
                Id = Guid.NewGuid(),
                Featured = true,
                Image = "https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/1.jpg",
                AuthorImage = "https://avatars.githubusercontent.com/u/25960932?s=96&v=4",
                Author = "Dominik Zettl",
                Title = ".NET self hosted workers",
                Date = DateTime.Now,
                Category = "Coding",
                Tags = new List<string>() { ".NET", "Worker" },
                HtmlText = "| Syntax | Description |\r\n| ----------- | ----------- |\r\n| Header | Title |\r\n| Paragraph | Text |"
            },
            new ArticleDetailVM()
            {
                Id = Guid.NewGuid(),
                Featured = true,
                Image = "https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/2.jpg",
                AuthorImage = "https://avatars.githubusercontent.com/u/25960932?s=96&v=4",
                Author = "Dominik Zettl",
                Title = ".NET CLI Framework",
                Date = DateTime.Now,
                Category = "Coding",
                Tags = new List<string>() { ".NET", "CLI", "Framework" },
                HtmlText = "```csharp\r\npublic class Test\r\n{\r\npublic static int Test() {} \r\n}\r\n```"
            },
            new ArticleDetailVM()
            {
                Id = Guid.NewGuid(),
                Image = "https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/3.jpg",
                AuthorImage = "https://avatars.githubusercontent.com/u/25960932?s=96&v=4",
                Author = "Dominik Zettl",
                Title = "Movie Posters",
                Date = DateTime.Now,
                Category = "Movie",
                Tags = new List<string>() { "Movie", "LED", "DIY" }
            },
            new ArticleDetailVM()
            {
                Id = Guid.NewGuid(),
                Image = "https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/4.jpg",
                AuthorImage = "https://avatars.githubusercontent.com/u/25960932?s=96&v=4",
                Author = "Dominik Zettl",
                Title = "Plant Watering System",
                Date = DateTime.Now,
                Category = "Project",
                Tags = new List<string>() { "Plant Watering System", "Raspberry" }
            },
            new ArticleDetailVM()
            {
                Id = Guid.NewGuid(),
                Image = "https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/5.jpg",
                AuthorImage = "https://avatars.githubusercontent.com/u/25960932?s=96&v=4",
                Author = "Dominik Zettl",
                Title = ".NET Maui Test",
                Date = DateTime.Now,
                Category = "Coding",
                Tags = new List<string>() { ".NET", "Maui" }
            },
            new ArticleDetailVM()
            {
                Id = Guid.NewGuid(),
                Image = "https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/6.jpg",
                AuthorImage = "https://avatars.githubusercontent.com/u/25960932?s=96&v=4",
                Author = "Dominik Zettl",
                Title = "Steam Link on Rapsberry",
                Date = DateTime.Now,
                Category = "Gaming",
                Tags = new List<string>() { "Steam", "Raspberry", "Gaming"  }
            }
        };

    public Task<ArticleDetailVM?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var vm = _articles.FirstOrDefault(x => x.Id == id);
        if (vm is null)
            return Task.FromResult((ArticleDetailVM?)null);

        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        vm.HtmlText = Markdown.ToHtml(vm.HtmlText, pipeline);
        return Task.FromResult((ArticleDetailVM?)vm);
    }

    public Task<List<ArticleVM>> Search(ArticleFilter filter, CancellationToken cancellationToken = default)
    {
        if (filter.Category?.Equals("all", StringComparison.OrdinalIgnoreCase) ?? false)
            filter.Category = null;

        return Task.FromResult(_articles
            .Where(x => filter.Text is null || x.Title.Contains(filter.Text, StringComparison.OrdinalIgnoreCase))
            .Where(x => filter.Category is null || x.Category.Equals(filter.Category, StringComparison.OrdinalIgnoreCase))
            .Cast<ArticleVM>()
            .ToList());
    }

    public Task<List<string>> GetAllCategories(CancellationToken cancellationToken = default)
    {
        var all = "All";
        var categories = _articles
            .Select(x => x.Category)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        categories.Insert(0, all);
        return Task.FromResult(categories);
    }
}

public class ArticleFilter
{
    public string? Text { get; set; }
    public string? Category { get; set; }

    public static ArticleFilter Default => new();
}
