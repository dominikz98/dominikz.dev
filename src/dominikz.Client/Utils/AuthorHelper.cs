using dominikz.Domain.Enums.Blog;

namespace dominikz.Client.Utils;

public static class AuthorHelper
{
    public static string GetNameBySource(ArticleSourceEnum source)
        => source switch
        {
            ArticleSourceEnum.Dz => "Dominik Zettl",
            ArticleSourceEnum.Medlan => "Markus Liebl",
            ArticleSourceEnum.Noobit => "Tobias Haimerl",
            _ => string.Empty
        };
}