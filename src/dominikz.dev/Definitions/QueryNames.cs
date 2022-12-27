namespace dominikz.dev.Definitions;

public static class QueryNames
{
    public static class Blog
    {
        public const string Search = "search";
        public const string Category = "category";
        public const string Source = "source";
    }

    public static class Media
    {
        public const string Search = "search";
        public const string Category = "category";
        
        public static class Movie
        {
            public const string Genre = "m_genre";
        }
        
        public static class Book
        {
            public const string Genre = "b_genre";
            public const string Language = "b_language";
        }
        
        public static class Game
        {
            public const string Genre = "g_genre";
            public const string Platform = "g_platform";       
        }
    }
}