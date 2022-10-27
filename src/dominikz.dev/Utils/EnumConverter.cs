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

        else if (value is MovieGenresFlags movieGenre)
            return ToString(movieGenre);

        else if (value is ArticleCategoryEnum articleCategory)
            return ToString(articleCategory);

        else if (value is FoodUnitEnum foodUnit)
            return ToString(foodUnit);

        else if (value is GamePlatformEnum platform)
            return ToString(platform);

        else if (value is GameGenresFlags gameGenre)
            return ToString(gameGenre);

        else if (value is BookLanguageEnum language)
            return ToString(language);

        else if (value is BookGenresFlags bookGenre)
            return ToString(bookGenre);

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
            //MediaCategoryEnum.Series => "🍿 Series",
            MediaCategoryEnum.Movie => "🎞 Movie",
            MediaCategoryEnum.Book => "📖 Book",
            MediaCategoryEnum.Game => "🎮 Game",
            _ => Default
        };

    private static string ToString(MovieGenresFlags genre)
        => genre switch
        {
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
            MovieGenresFlags.ALL or _ => Default
        };

    private static string ToString(ArticleCategoryEnum category)
        => category switch
        {
            ArticleCategoryEnum.ALL => ALL,
            ArticleCategoryEnum.Coding => "💡 Coding",
            ArticleCategoryEnum.Movie => "🎞 Movie",
            ArticleCategoryEnum.Project => "🚀 Project",
            ArticleCategoryEnum.Gaming => "🎮 Gaming",
            ArticleCategoryEnum.Travel => "✈ Travel",
            ArticleCategoryEnum.Birds => "🐤 Birds",
            ArticleCategoryEnum.Thoughts => "💡 Thoughts",
            _ => Default
        };

    private static string ToString(GamePlatformEnum platform)
        => platform switch
        {
            GamePlatformEnum.PC => "🖥 PC",
            GamePlatformEnum.PS4 => "🎮 PS4",
            GamePlatformEnum.Switch => "🎮 Nintendo Switch",
            _ => Default
        };

    private static string ToString(GameGenresFlags genre)
        => genre switch
        {
            GameGenresFlags.ALL => ALL,
            GameGenresFlags.VirtualReality => "👓 VR",
            GameGenresFlags.Indie => "🕹 Indie",
            GameGenresFlags.Survival => "🎒 Survival",
            GameGenresFlags.Horror => "👻 Horror",
            GameGenresFlags.Action => "📣 Action",
            GameGenresFlags.Puzzle => "🧩 Puzzle",
            GameGenresFlags.OpenWorld => "🌎 Open World",
            GameGenresFlags.Adventure => "🗺 Adventure",
            GameGenresFlags.Shooter => "🔫 Shooter",
            GameGenresFlags.RealTime => "🕑 Realtime",
            GameGenresFlags.Strategy => "✏ Strategy",
            GameGenresFlags.Tactic => "🗒 Tactic",
            GameGenresFlags.Rpg => "🌲 RPG",
            GameGenresFlags.Sandbox => "📦 Sandbox",
            GameGenresFlags.Simulation => "⌨ Simulation",
            GameGenresFlags.Stealth => "🥷 Stealth",
            GameGenresFlags.Racing => "🚗 Racing",
            GameGenresFlags.Construction => "🚧 Construction",
            GameGenresFlags.JumpNRun => "🔘 Jump and Run",
            GameGenresFlags.BattleRoyal => "🏆 Battle Royal",
            GameGenresFlags.HackAndSlay => "⚔ Hack and Slay",
            GameGenresFlags.ClickAndPoint => "🖱 Click & Point",
            GameGenresFlags.Party => "🎈 Party",
            _ => Default
        };

    private static string ToString(BookGenresFlags genre)
        => genre switch
        {
            BookGenresFlags.ALL => ALL,
            BookGenresFlags.Crime => "🎈 Crime",
            BookGenresFlags.Fantasy => "🦄 Fantasy",
            BookGenresFlags.Adventure => "🗺 Adventure",
            BookGenresFlags.Horror => "👻 Horror",
            BookGenresFlags.SciFi => "🚀 SciFi",
            BookGenresFlags.Novel => "📕 Novel",
            BookGenresFlags.Thriller => "🧟 Thriller",
            BookGenresFlags.Dystopia => "💣 Dystopia",
            BookGenresFlags.NonFiction => "📚 Non-Fiction",
            BookGenresFlags.Advising => "🎓 Advising",
            BookGenresFlags.Romance => "♥ Romance",
            BookGenresFlags.Humor => "🎤 Comedy",
            _ => Default
        };

    private static string ToString(BookLanguageEnum language)
        => language switch
        {
            BookLanguageEnum.German => "German",
            BookLanguageEnum.English => "English",
            _ => Default
        };
}
