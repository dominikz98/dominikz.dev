using AutoMapper;
using dominikz.Api.Mapper.Actions;
using dominikz.Api.Models;
using dominikz.Common.Enumerations;
using dominikz.Endpoints.ViewModels;
using System.Linq;
using System.Xml.Linq;

namespace dominikz.Api.Mapper
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<Activity, VMActivity>()
                   .ForMember(destination => destination.Tags, member => member.MapFrom(src => src.Tags.Select(x => x.Name)));

            CreateMap<Blogpost, VMBlogpostPreview>()
                .IncludeBase<Activity, VMActivity>()
                .AfterMap<AttachBlogBannerUrlAction>();

            CreateMap<Blogpost, VMBlogpost>()
                .IncludeBase<Blogpost, VMBlogpostPreview>()
                .AfterMap<AttachBlogImageUrlAction>();

            CreateMap<Movie, VMMoviePreview>()
                .ForMember(destination => destination.Tags, member => member.MapFrom(src => src.Tags.Where(x => x.Type != TagType.MovieCategory).Select(x => x.Name)))
                .ForMember(destination => destination.Categories, member => member.MapFrom(src => src.Categories.Where(x => x.Type == TagType.MovieCategory).Select(x => x.Name)))
                .IncludeBase<Activity, VMActivity>()
                .AfterMap<AttachMovieThumbnailUrlAction>();

            CreateMap<Movie, VMMovie>()
                .IncludeBase<Movie, VMMoviePreview>()
                .AfterMap<AttachMovieImageUrlAction>();

            CreateMap<MovieRating, VMMovieRating>();
            CreateMap<MovieStar, VMMovieStar>();

            CreateMap<PenAndPaper, VMPenAndPaper>()
                .IncludeBase<Activity, VMActivity>();

            CreateMap<PenAndPaper, VMPenAndPaperAdventure>()
                .IncludeBase<PenAndPaper, VMPenAndPaper>();

            CreateMap<Script, VMScript>()
                .IncludeBase<Activity, VMActivity>();
        }
    }
}
