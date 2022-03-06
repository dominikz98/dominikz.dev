using System.Collections.Generic;

namespace dominikz.Endpoints.ViewModels
{
    public class VMPodcast
    {
        public string Description { get; set; }
        public List<VMPodcastEpisode> Episodes { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
        public string Instagram { get; set; }
        public string ITunes { get; set; }
        public string Spotify { get; set; }
        public string RSS { get; set; }
        public List<string> Categories { get; set; }

        public VMPodcast()
        {
            Episodes = new();
            Categories = new();
        }
    }
}
