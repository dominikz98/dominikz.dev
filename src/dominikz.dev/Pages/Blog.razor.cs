namespace dominikz.dev.Pages;

public partial class Blog
{
    private string? _search;

    private List<Article> _articles = new()
    {
        new Article("https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/1.jpg", "https://avatars.githubusercontent.com/u/25960932?s=96&v=4", "Dominik Zettl", ".NET self hosted workers", DateTime.Now, "Create your own worker environment from skratch", "Coding", new List<string>() { ".NET", "Worker" }),
        new Article("https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/2.jpg", "https://avatars.githubusercontent.com/u/25960932?s=96&v=4", "Dominik Zettl", ".NET CLI Framework", DateTime.Now, "Use Clizer to easily call your services by cli", "Coding", new List<string>() { ".NET", "CLI", "Framework" }),
        new Article("https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/3.jpg", "https://avatars.githubusercontent.com/u/25960932?s=96&v=4", "Dominik Zettl", "Movie Posters", DateTime.Now, "Possible to make cheap and custom movie posters like displate?", "Movie", new List<string>() { "Movie", "LED", "DIY" }),
        new Article("https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/4.jpg", "https://avatars.githubusercontent.com/u/25960932?s=96&v=4", "Dominik Zettl", "Plant Watering System", DateTime.Now, "I killed all my plants, lets make something different", "Project", new List<string>() { "Plant Watering System", "Raspberry" }),
        new Article("https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/5.jpg", "https://avatars.githubusercontent.com/u/25960932?s=96&v=4", "Dominik Zettl", ".NET Maui Test", DateTime.Now, "MVVM is anoying, also in .NET Maui", "Coding", new List<string>() { ".NET", "Maui" }),
        new Article("https://material-components.github.io/material-components-web-catalog/static/media/photos/3x2/6.jpg", "https://avatars.githubusercontent.com/u/25960932?s=96&v=4", "Dominik Zettl", "Steam Link on Rapsberry", DateTime.Now, "Play PC Games from Steam on TV at the Couch", "Gaming", new List<string>() { "Steam", "Raspberry", "Gaming" })
    };
}

public record Article(string Image, string AuthorImage, string Author, string Title, DateTime Date, string SubTitle, string Category, List<string> Tags);