namespace dominikz.Endpoints
{
    public static class Endpoints
    {
        public static class Activities
        {
            public static readonly string Get = "/api/activities";
        }

        public static class Blog
        {
            public static readonly string Get = "/api/blog";
        }

        public static class Movies
        {
            public static readonly string Get = "/api/movies";
            public static readonly string GetCategories = "/api/movies/categories";
            public static readonly string GetStars = "/api/movies/stars";
            public static readonly string GetLatest = "/api/movies/latest";
            public static readonly string GetFeatured = "/api/movies/featured";
        }

        public static class Podcast
        {
            public static readonly string Get = "/api/podcast";
        }

        public static class PenAndPaper
        {
            public static readonly string Get = "/api/pap";
        }

        public static class Scripts
        {
            public static readonly string CSharp = "/api/scripts/csharp";
            public static readonly string SQL = "/api/scripts/sql";
        }
    }
}
