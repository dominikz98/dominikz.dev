using System;

namespace dominikz.Endpoints.ViewModels
{
    public class VMPodcastEpisode
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Publication { get; set; }
        public string ImageUrl { get; set; }
        public string Instagram { get; set; }
        public string ITunes { get; set; }
        public string Spotify { get; set; }
        public string RSS { get; set; }
        public int Duration { get; set; }
    }
}
