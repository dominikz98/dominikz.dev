using dominikz.kernel.ViewModels;

namespace dominikz.dev.Utils;

public class EnumConverter
{
    public const string ALL = "All";
    public const string Default = "Unknown";
    // https://emojipedia.org/

    public static string ToString<TEnum>(List<TEnum> values) where TEnum : struct, Enum
        => string.Join(", ", values.Select(x => ToString(x)));

    public static string ToString<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        if (value is MediaCategoryEnum mediaCategory)
            return ToString(mediaCategory);

        else if (value is MediaGenre mediaGenre)
            return ToString(mediaGenre);

        else if (value is ArticleCategoryEnum articleCategory)
            return ToString(articleCategory);

        else
            return value.ToString()!;
    }

    private static string ToString(MediaCategoryEnum category)
        => category switch
        {
            MediaCategoryEnum.ALL => ALL,
            MediaCategoryEnum.SERIES => "🍿 Series",
            MediaCategoryEnum.MOVIE => "🎞 Movie",
            MediaCategoryEnum.BOOK => "📖 Book",
            MediaCategoryEnum.GAME => "🎮 Game",
            _ => Default
        };

    private static string ToString(MediaGenre genre)
        => genre switch
        {
            MediaGenre.ALL => ALL,
            MediaGenre.SUPERHERO => "🦸 Superhero",
            MediaGenre.HORROR => "👻 Horror",
            MediaGenre.DRAMA => "🎭 Drama",
            MediaGenre.COOP => "🤲 Coop",
            MediaGenre.TRASH => "🗑 Trash",
            _ => Default
        };

    private static string ToString(ArticleCategoryEnum category)
        => category switch
        {
            ArticleCategoryEnum.ALL => ALL,
            ArticleCategoryEnum.CODING => "💡 Coding",
            ArticleCategoryEnum.MOVIE => "🎞 Movie",
            ArticleCategoryEnum.PROJECT => "🚀 Project",
            ArticleCategoryEnum.GAMING => "🎮 Gaming",
            _ => Default
        };
}
