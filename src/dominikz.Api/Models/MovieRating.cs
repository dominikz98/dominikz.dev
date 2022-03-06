namespace dominikz.Api.Models
{
    public class MovieRating
    {
        public int Id { get; set; }
        public int Actors { get; set; }
        public int Ambience { get; set; }
        public int Music { get; set; }
        public int Plot { get; set; }
        public int Regie { get; set; }

        public Movie Movie { get; set; }
    }
}
