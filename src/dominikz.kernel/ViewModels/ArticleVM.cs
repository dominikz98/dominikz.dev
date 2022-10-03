namespace dominikz.kernel.ViewModels;

public class ArticleVM
{
    public Guid Id { get; set; }
    public bool Featured { get; set; }
    public string Image { get; set; }
    public string AuthorImage { get; set; }
    public string Author { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public string Category { get; set; }

    public ArticleVM()
    {
        Image = string.Empty;
        AuthorImage = string.Empty;
        Author = string.Empty;
        Title = string.Empty;
        Category = string.Empty;
    }
}

public class ArticleDetailVM : ArticleVM
{
    public List<string> Tags { get; set; }
    public string HtmlText { get; set; }

    public ArticleDetailVM()
    {
        Tags = new();
        HtmlText = string.Empty;
    }
}
