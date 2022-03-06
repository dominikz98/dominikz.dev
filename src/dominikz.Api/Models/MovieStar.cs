using dominikz.Common.Enumerations;
using System.Collections.Generic;

namespace dominikz.Api.Models
{
    public class MovieStar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }
        public MovieJob Job { get; set; }
        public string ProfilePictureUrl { get; set; }

        public ICollection<Movie> Movies { get; set; }
    }
}
