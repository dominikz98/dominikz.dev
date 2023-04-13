using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Enums.Cookbook;
using dominikz.Domain.Enums.Movies;

namespace dominikz.Client.Utils;

public static class EnumFormatter
{
    private const string Default = "Unknown";

    public static string ToString<TEnum>(IReadOnlyCollection<TEnum> values) where TEnum : struct
        => string.Join(", ", values.Select(ToString));

    public static string ToString<TEnum>(TEnum value) where TEnum : struct
    {
        if (value is MovieGenresFlags movieGenre)
            return ToString(movieGenre);
        if (value is ArticleCategoryEnum articleCategory)
            return ToString(articleCategory);
        if (value is ArticleSourceEnum articleSource)
            return ToString(articleSource);
        if (value is RecipeFlags recipeFlag)
            return ToString(recipeFlag);
        if (value is RecipeType recipeType)
            return ToString(recipeType);
        
        return value.ToString()!;
    }

    private static string ToString(MovieGenresFlags genre)
        => genre switch
        {
            MovieGenresFlags.All => string.Empty,
            MovieGenresFlags.Horror => "👻 Horror",
            MovieGenresFlags.Drama => "🎭 Drama",
            MovieGenresFlags.Mystery => "🔮 Mystery",
            MovieGenresFlags.Thriller => "🧟 Thriller",
            MovieGenresFlags.Action => "📣 Action",
            MovieGenresFlags.Adventure => "🗺 Adventure",
            MovieGenresFlags.Fantasy => "🦄 Fantasy",
            MovieGenresFlags.Comedy => "🎤 Comedy",
            MovieGenresFlags.Western => "🐴 Western",
            MovieGenresFlags.SciFi => "🚀 SciFi",
            MovieGenresFlags.Animation => "🐇 Animation",
            MovieGenresFlags.Crime => "🎈 Crime",
            MovieGenresFlags.Musical => "🎻 Musical",
            MovieGenresFlags.War => "🔫 War",
            MovieGenresFlags.Romance => "♥ Romance",
            MovieGenresFlags.Biography => "🎩 Biography",
            MovieGenresFlags.History => "📙 History",
            MovieGenresFlags.Family => "👪 Family",
            _ => Default
        };

    private static string ToString(ArticleCategoryEnum category)
        => category switch
        {
            ArticleCategoryEnum.Coding => "💾 Coding",
            ArticleCategoryEnum.Movie => "🎞 Movie",
            ArticleCategoryEnum.Project => "🚀 Project",
            ArticleCategoryEnum.Gaming => "🎮 Gaming",
            ArticleCategoryEnum.Travel => "✈ Travel",
            ArticleCategoryEnum.Birds => "🐤 Birds",
            ArticleCategoryEnum.Thoughts => "💡 Thoughts",
            ArticleCategoryEnum.Music => "🎸 Music",
            ArticleCategoryEnum.Finance => "💵 Finance",
            ArticleCategoryEnum.Shopping => "🛒 Shopping",
            ArticleCategoryEnum.Leisure => "🎭️ Leisure",
            _ => Default
        };

    private static string ToString(ArticleSourceEnum sourceEnum)
        => sourceEnum switch
        {
            ArticleSourceEnum.Dz => "Dominik",
            ArticleSourceEnum.Medlan => "Medlan",
            ArticleSourceEnum.Noobit => "Noobit",
            _ => Default
        };

    private static string ToString(RecipeFlags recipe)
        => recipe switch
        {
            RecipeFlags.Vegetarian => "🍏 Vegetarian",
            RecipeFlags.Vegan => "🥬 Vegan",
            RecipeFlags.GlutenFree => "🌾 Gluten Free",
            RecipeFlags.LactoseFree => "🥛 Lactose Free",
            _ => Default
        };

    private static string ToString(RecipeType recipe)
        => recipe switch
        {
            RecipeType.Starter => "🍲 Starter",
            RecipeType.Main => "🥗 Main",
            RecipeType.Dessert => "🍨 Dessert",
            RecipeType.Cake => "🧁 Cake",
            _ => throw new ArgumentOutOfRangeException(nameof(recipe), recipe, null)
        };
    
    public static string ToIcon(RecipeType recipe)
        => recipe switch
        {
            RecipeType.Starter => "🍲",
            RecipeType.Main => "🥗",
            RecipeType.Dessert => "🍨",
            RecipeType.Cake => "🧁",
            _ => throw new ArgumentOutOfRangeException(nameof(recipe), recipe, null)
        };
    
    public static string ToText(RecipeType recipe)
        => recipe switch
        {
            RecipeType.Starter => "Starter",
            RecipeType.Main => "Main",
            RecipeType.Dessert => "Dessert",
            RecipeType.Cake => "Cake",
            _ => throw new ArgumentOutOfRangeException(nameof(recipe), recipe, null)
        };


    public static string ToIcon(RecipeFlags recipe)
        => recipe switch
        {
            RecipeFlags.Vegetarian => "🍏",
            RecipeFlags.Vegan => "🥬",
            RecipeFlags.GlutenFree => "🌾",
            RecipeFlags.LactoseFree => "🥛",
            _ => Default
        };
    
    public static string ToText(RecipeFlags recipe)
        => recipe switch
        {
            RecipeFlags.Vegetarian => "Vegetarian",
            RecipeFlags.Vegan => "Vegan",
            RecipeFlags.GlutenFree => "Gluten Free",
            RecipeFlags.LactoseFree => "Lactose Free",
            _ => Default
        };
}