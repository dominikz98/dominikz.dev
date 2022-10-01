using dominikz.kernel.ViewModels;

namespace dominikz.kernel.Endpoints;

public class ArticlesEndpoints
{
    private readonly List<ArticleDetailVM> _articles = new()
        {
            new ArticleDetailVM()
            {
                Id = Guid.NewGuid(),
                Image = "https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/1.jpg",
                AuthorImage = "https://avatars.githubusercontent.com/u/25960932?s=96&v=4",
                Author = "Dominik Zettl",
                Title = ".NET self hosted workers",
                Date = DateTime.Now,
                Category = "Coding",
                Tags = new List<string>() { ".NET", "Worker" }
            },
            new ArticleDetailVM()
            {
                Id = Guid.NewGuid(),
                Image = "https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/2.jpg",
                AuthorImage = "https://avatars.githubusercontent.com/u/25960932?s=96&v=4",
                Author = "Dominik Zettl",
                Title = ".NET CLI Framework",
                Date = DateTime.Now,
                Category = "Coding",
                Tags = new List<string>() { ".NET", "CLI", "Framework" }
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

    public Task<List<ArticleVM>> Get(CancellationToken cancellationToken)
        => Task.FromResult(_articles.Cast<ArticleVM>().ToList());

    public Task<ArticleDetailVM?> GetById(Guid id, CancellationToken cancellationToken)
        => Task.FromResult(_articles.FirstOrDefault(x => x.Id == id));
}
