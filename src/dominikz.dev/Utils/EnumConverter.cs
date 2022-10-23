using dominikz.kernel.Contracts;

namespace dominikz.dev.Utils;

public class EnumConverter
{
    public const string ALL = "All";
    public const string Default = "Unknown";
    // https://emojipedia.org/

    public static string ToString<TEnum>(IReadOnlyCollection<TEnum> values) where TEnum : struct, Enum
        => string.Join(", ", values.Select(x => ToString(x)));

    public static string ToString<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        if (value is MediaCategoryEnum mediaCategory)
            return ToString(mediaCategory);

        else if (value is MovieGenreFlags mediaGenre)
            return ToString(mediaGenre);

        else if (value is ArticleCategoryEnum articleCategory)
            return ToString(articleCategory);

        else if (value is FoodUnitEnum foodUnit)
            return ToString(foodUnit);

        else
            return value.ToString()!;
    }

    private static string ToString(FoodUnitEnum unit)
       => unit switch
       {
           FoodUnitEnum.Pieces => "pc.",
           FoodUnitEnum.Grams => "g",
           FoodUnitEnum.Liter => "l",
           _ => Default
       };

    private static string ToString(MediaCategoryEnum category)
        => category switch
        {
            MediaCategoryEnum.ALL => ALL,
            MediaCategoryEnum.Series => "🍿 Series",
            MediaCategoryEnum.Movie => "🎞 Movie",
            MediaCategoryEnum.Book => "📖 Book",
            MediaCategoryEnum.Game => "🎮 Game",
            _ => Default
        };

    private static string ToString(MovieGenreFlags genre)
        => genre switch
        {
            MovieGenreFlags.Horror => "👻 Horror",
            MovieGenreFlags.Drama => "🎭 Drama",
            MovieGenreFlags.Mystery => "🔮 Mystery",
            MovieGenreFlags.Thriller => "🧟 Thriller",
            MovieGenreFlags.Action => "📣 Action",
            MovieGenreFlags.Adventure => "🗺 Adventure",
            MovieGenreFlags.Fantasy => "🦄 Fantasy",
            MovieGenreFlags.Comedy => "🎤 Comedy",
            MovieGenreFlags.Western => "🐴 Western",
            MovieGenreFlags.SciFi => "🚀 SciFi",
            MovieGenreFlags.Animation => "🐇 Animation",
            MovieGenreFlags.Crime => "🎈 Crime",
            MovieGenreFlags.Musical => "🎻 Musical",
            MovieGenreFlags.War => "🔫 War",
            MovieGenreFlags.Romance => "♥ Romance",
            MovieGenreFlags.Biography => "🎩 Biography",
            MovieGenreFlags.History => "📙 History",
            MovieGenreFlags.ALL or _ => Default
        };

    //MediaGenreFlags.Horror => "👻 Horror",
    //MediaGenreFlags.Drama => "🎭 Drama",

    private static string ToString(ArticleCategoryEnum category)
        => category switch
        {
            ArticleCategoryEnum.ALL => ALL,
            ArticleCategoryEnum.Coding => "💡 Coding",
            ArticleCategoryEnum.Movie => "🎞 Movie",
            ArticleCategoryEnum.Project => "🚀 Project",
            ArticleCategoryEnum.Gaming => "🎮 Gaming",
            _ => Default
        };
}
