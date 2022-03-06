using dominikz.Common.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dominikz.Api.Models
{
    public class Movie : Activity
    {
        public string KeyWord { get; set; }
        public string MDContent { get; set; }
        public string Thumbnail { get; set; }
        public string YoutubeTrailerId { get; set; }
        public MovieProvider Provider { get; set; }
        public TimeSpan Runtime { get; set; }
        public DateTime Publication { get; set; }
        public DateTime Watched { get; set; }
        public MovieUSK USK { get; set; }

        public int RatingId { get; set; }
        public MovieRating Rating { get; set; }
        public double Score { get => (Rating?.Actors + Rating?.Ambience + Rating?.Music + Rating?.Plot + Rating?.Regie) / 5.0 ?? 0; }

        public ICollection<MovieStar> Stars { get; set; }
        public ICollection<ItemTag> Categories { get => Tags.Where(x => x.Type == TagType.MovieCategory).ToList(); }
    }
}
