using dominikz.Common.Enumerations;

namespace dominikz.Endpoints.ViewModels
{
    public class VMMovieStar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }
        public MovieJob Job { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
