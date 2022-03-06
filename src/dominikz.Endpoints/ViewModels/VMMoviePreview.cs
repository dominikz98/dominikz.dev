using System.Collections.Generic;

namespace dominikz.Endpoints.ViewModels
{
    public class VMMoviePreview : VMActivity
    {
        public string Thumbnail { get; set; }
        public List<string> Categories { get; set; }
        public double Score { get; set; }

        public VMMoviePreview()
        {
            Categories = new();
        }
    }
}
