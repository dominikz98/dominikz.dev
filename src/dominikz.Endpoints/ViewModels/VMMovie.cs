using dominikz.Common.Enumerations;
using System;
using System.Collections.Generic;

namespace dominikz.Endpoints.ViewModels
{
    public class VMMovie : VMMoviePreview
    {
        public string KeyWord { get; set; }
        public string MDContent { get; set; }
        public string YoutubeTrailerId { get; set; }
        public string ImagesUrl { get; set; }
        public MovieProvider Provider { get; set; }
        public TimeSpan Runtime { get; set; }
        public DateTime Publication { get; set; }
        public DateTime Watched { get; set; }
        public MovieUSK USK { get; set; }

        public VMMovieRating Rating { get; set; }
        public List<VMMovieStar> Stars { get; set; }

        public VMMovie()
        {
            Stars = new();
        }
    }
}
