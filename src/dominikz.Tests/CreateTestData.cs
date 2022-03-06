using dominikz.Api;
using dominikz.Api.Models;
using dominikz.Common.Enumerations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace dominikz.Tests
{
    public class CreateTestData
    {
        [Fact]
        public async Task Execute()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(@"Data Source=C:\Users\domin\Documents\dominikz.db")
                .Options;

            var context = new DataContext(options);

            // Tags
            var tags = CreateTags();
            await context.AddRangeAsync(tags);

            // Blogposts
            var blogposts = CreateBlogPosts(tags);
            await context.AddRangeAsync(blogposts);

            // Movies
            var movies = CreateMovies(tags);
            await context.AddRangeAsync(movies);

            // PenAndPaper
            var paps = CreatePAPs(tags);
            await context.AddRangeAsync(paps);

            // Scripts
            var scripts = CreateScripts(tags);
            await context.AddRangeAsync(scripts);

            await context.SaveChangesAsync();
        }

        private static List<ItemTag> CreateTags()
            => new()
            {
                new ItemTag() { Name = "Kontrovers" },
                new ItemTag() { Name = "Biber" },
                new ItemTag() { Name = "Thornado" },
                new ItemTag() { Name = "Beschissen" },
                new ItemTag() { Name = "Religion" },
                new ItemTag() { Name = "Butter" },
                new ItemTag() { Name = "Geometrie" },
                new ItemTag() { Name = "Erdoberfläche" },
                new ItemTag() { Name = "Klimawandel" },
                new ItemTag() { Name = "Lüge" },
                new ItemTag() { Name = "Stuhlgang" },
                new ItemTag() { Name = "Vielfliegermeilen" },
                new ItemTag() { Name = "Horror", Type = TagType.MovieCategory },
                new ItemTag() { Name = "Erotik", Type = TagType.MovieCategory },
                new ItemTag() { Name = "Sci-Fi", Type = TagType.MovieCategory },
                new ItemTag() { Name = "Kinderfilm", Type = TagType.MovieCategory },
                new ItemTag() { Name = "Sonntagsfilm", Type = TagType.MovieCategory },
                new ItemTag() { Name = "Drama", Type = TagType.MovieCategory },
                new ItemTag() { Name = "Mystery", Type = TagType.MovieCategory },
                new ItemTag() { Name = "Pen and Paper" },
                new ItemTag() { Name = "TEARS" },
                new ItemTag() { Name = "Gammaslayers" },
                new ItemTag() { Name = "Morriton Manor" },
                new ItemTag() { Name = "Script" },
                new ItemTag() { Name = "C#" },
                new ItemTag() { Name = "EF Core" }
            };

        private static List<Blogpost> CreateBlogPosts(List<ItemTag> tags)
            => new()
            {
                new Blogpost()
                {
                    Title = "Papst in Wahrheit Buddhist",
                    Description = "Zuviel mit Buddha eingerieben?!?!",
                    Banner = "blue.jpg",
                    Category = ActivityCategory.Blog,
                    Release = new DateTime(2021, 8, 1),
                    MDContent = "*LOL*",
                    Tags = new List<ItemTag>()
                    {
                        tags.Single(x => x.Name == "Religion"),
                        tags.Single(x => x.Name == "Butter")
                    }
                },
                new Blogpost()
                {
                    Title = "Erde rund sondern dreieckig",
                    Description = "Durch geleakte Bilder der ISS bestätigt!",
                    Banner = "blurred.jpg",
                    Category = ActivityCategory.Blog,
                    Release = new DateTime(2021, 9, 11),
                    MDContent = "*LOL*",
                    Tags = new List<ItemTag>()
                    {
                        tags.Single(x => x.Name == "Geometrie"),
                        tags.Single(x => x.Name == "Erdoberfläche")
                    }
                },
                new Blogpost()
                {
                    Title = "TreibHAUSgas",
                    Description = "Wie kanns in die Atmosphäre gelangen wenns im Haus ist?",
                    Banner = "bubbles.jpg",
                    Category = ActivityCategory.Blog,
                    Release = new DateTime(2021, 10, 3),
                    MDContent = "*LOL*",
                    Tags = new List<ItemTag>()
                    {
                        tags.Single(x => x.Name == "Klimawandel"),
                        tags.Single(x => x.Name == "Lüge")
                    }
                },
                new Blogpost()
                {
                    Title = "Ufosichtung bestätigt!",
                    Description = "Flugzeug wift Kot über Festland ab!",
                    Banner = "cracks.jpg",
                    Category = ActivityCategory.Blog,
                    Release = new DateTime(2021, 10, 14),
                    MDContent = "*LOL*",
                    Tags = new List<ItemTag>()
                    {
                        tags.Single(x => x.Name == "Stuhlgang"),
                        tags.Single(x => x.Name == "Vielfliegermeilen")
                    }
                }
            };

        private static List<Movie> CreateMovies(List<ItemTag> tags)
            => new()
            {
                new Movie()
                {
                    Title = "Zombeavers",
                    Description = "Horrorerotik mit Biebern",
                    Thumbnail = "zombeavers.jpg",
                    Category = ActivityCategory.Movie,
                    Release = new DateTime(2021, 10, 15),
                    Watched = new DateTime(2021, 10, 14),
                    MDContent = "*LOL*",
                    KeyWord = "Erotisch",
                    Provider = MovieProvider.DVD,
                    Publication = new DateTime(2014, 11, 6),
                    USK = MovieUSK.AK18,
                    YoutubeTrailerId = "L6I5hl1w0eg",
                    Runtime = new TimeSpan(1, 25, 0),
                    Stars = new List<MovieStar>()
                    {
                        new MovieStar
                        {
                            Surename = "Cortney",
                            Name = "Palm",
                            Job = MovieJob.Actor,
                            ProfilePictureUrl = "https://upload.wikimedia.org/wikipedia/commons/f/f6/Tara_Reid_%286961714509%29_%28modified%29.jpg"
                        },
                        new MovieStar
                        {
                            Surename = "Jordan",
                            Name = "Rubin",
                            Job = MovieJob.Director
                        },
                        new MovieStar
                        {
                            Surename = "Al",
                            Name = "Kaplan",
                            Job = MovieJob.Writer
                        },
                        new MovieStar
                        {
                            Surename = "Jon",
                            Name = "Kaplan",
                            Job = MovieJob.Musician
                        }
                    },
                    Tags = new List<ItemTag>()
                    {
                        tags.Single(x => x.Name == "Kontrovers"),
                        tags.Single(x => x.Name == "Biber"),
                        tags.Single(x => x.Name == "Horror"),
                        tags.Single(x => x.Name == "Erotik")
                    },
                    Rating = new MovieRating()
                    {
                        Actors = 5,
                        Ambience = 5,
                        Plot = 5,
                        Music = 5,
                        Regie = 5
                    }
                },
                new Movie()
                {
                    Title = "Sharknado – Genug gesagt!",
                    Description = "Wettervorhersage: Hai",
                    Thumbnail = "sharknado.jpg",
                    Category = ActivityCategory.Movie,
                    Release = new DateTime(2021, 10, 13),
                    Watched = new DateTime(2021, 10, 12),
                    MDContent = "*LOL*",
                    KeyWord = "Fischig",
                    Provider = MovieProvider.Amazon,
                    Publication = new DateTime(213, 5, 1),
                    USK = MovieUSK.AK12,
                    YoutubeTrailerId = "WUomU9qgvx4",
                    Runtime = new TimeSpan(1, 26, 0),
                    Stars = new List<MovieStar>()
                        {
                                new MovieStar
                                {
                                    Surename = "Tara",
                                    Name = "Reid",
                                    Job = MovieJob.Actor,
                                },
                                new MovieStar
                                {
                                    Surename = "Anthony",
                                    Name = "C.",
                                    Job = MovieJob.Director
                                },
                                new MovieStar
                                {
                                    Surename = "Thunder",
                                    Name = "Levin",
                                    Job = MovieJob.Writer
                                },
                                new MovieStar
                                {
                                    Surename = "Ramin",
                                    Name = "Kousha",
                                    Job = MovieJob.Musician
                                }
                        },
                    Tags = new List<ItemTag>()
                        {
                                tags.Single(x => x.Name == "Thornado"),
                                tags.Single(x => x.Name == "Beschissen"),
                                tags.Single(x => x.Name == "Horror"),
                                tags.Single(x => x.Name == "Sci-Fi")
                        },
                    Rating = new MovieRating()
                    {
                        Actors = 4,
                        Ambience = 4,
                        Plot = 4,
                        Music = 4,
                        Regie = 4
                    }
                },
                new Movie()
                {
                    Title = "Sharknado 2",
                    Description = "Wettervorhersage: Hai 2",
                    Thumbnail = "sharknado2.jpg",
                    Category = ActivityCategory.Movie,
                    Release = new DateTime(2021, 10, 11),
                    Watched = new DateTime(2021, 10, 10),
                    MDContent = "*LOL*",
                    KeyWord = "Ekelhaft",
                    Provider = MovieProvider.Netflix,
                    Publication = new DateTime(2014, 11, 21),
                    USK = MovieUSK.AK12,
                    YoutubeTrailerId = "fWY4-u9Ewdc",
                    Runtime = new TimeSpan(1, 35, 0),
                    Stars = new List<MovieStar>()
                        {
                                new MovieStar
                                {
                                    Surename = "Tara",
                                    Name = "Reid",
                                    Job = MovieJob.Actor
                                },
                                new MovieStar
                                {
                                    Surename = "Anthony",
                                    Name = "C.",
                                    Job = MovieJob.Director
                                },
                                new MovieStar
                                {
                                    Surename = "Thunder",
                                    Name = "Levin",
                                    Job = MovieJob.Writer
                                },
                                new MovieStar
                                {
                                    Surename = "Chris",
                                    Name = "Cano",
                                    Job = MovieJob.Musician
                                }
                        },
                    Tags = new List<ItemTag>()
                        {
                                tags.Single(x => x.Name == "Thornado"),
                                tags.Single(x => x.Name == "Beschissen"),
                                tags.Single(x => x.Name == "Horror"),
                                tags.Single(x => x.Name == "Sci-Fi")
                        },
                    Rating = new MovieRating()
                    {
                        Actors = 3,
                        Ambience = 3,
                        Plot = 3,
                        Music = 3,
                        Regie = 3
                    }
                },
                new Movie()
                {
                    Title = "Max und Moritz Reloaded",
                    Description = "Scheinbar auch echt beschissen",
                    Thumbnail = "maxumoritz.jpg",
                    Category = ActivityCategory.Movie,
                    Release = new DateTime(2021, 10, 8),
                    Watched = new DateTime(2021, 10, 7),
                    MDContent = "*LOL*",
                    KeyWord = "Kindlich",
                    Provider = MovieProvider.TV,
                    Publication = new DateTime(2005, 2, 3),
                    USK = MovieUSK.AK16,
                    YoutubeTrailerId = "Wf42b5OE4MA",
                    Runtime = new TimeSpan(2, 1, 0),
                    Stars = new List<MovieStar>()
                        {
                                new MovieStar
                                {
                                    Surename = "Katy",
                                    Name = "Karrenbauer",
                                    Job = MovieJob.Actor
                                },
                                new MovieStar
                                {
                                    Surename = "Annette",
                                    Name = "Stefan",
                                    Job = MovieJob.Director
                                },
                                new MovieStar
                                {
                                    Surename = "Thomas",
                                    Name = "Frydetzki",
                                    Job = MovieJob.Director
                                },
                                new MovieStar
                                {
                                    Surename = "Laurens",
                                    Name = "Straub",
                                    Job = MovieJob.Writer
                                },
                                new MovieStar
                                {
                                    Surename = "Michael",
                                    Name = "Beckmann",
                                    Job = MovieJob.Musician
                                }
                        },
                    Tags = new List<ItemTag>()
                        {
                                tags.Single(x => x.Name == "Kinderfilm"),
                                tags.Single(x => x.Name == "Beschissen"),
                                tags.Single(x => x.Name == "Sonntagsfilm"),
                                tags.Single(x => x.Name == "Drama")
                        },
                    Rating = new MovieRating()
                    {
                        Actors = 2,
                        Ambience = 2,
                        Plot = 2,
                        Music = 2,
                        Regie = 2
                    }
                },
                new Movie()
                {
                    Title = "The Dark",
                    Description = "Ziemlich düster ...",
                    Thumbnail = "thedark.jpg",
                    Category = ActivityCategory.Movie,
                    Release = new DateTime(2021, 10, 1),
                    Watched = new DateTime(2021, 9, 18),
                    MDContent = "*LOL*",
                    KeyWord = "Gruselig",
                    Provider = MovieProvider.Netflix,
                    Publication = new DateTime(2005, 4, 13),
                    USK = MovieUSK.AK18,
                    YoutubeTrailerId = "qA55AznXS2M",
                    Runtime = new TimeSpan(1, 33, 0),
                    Stars = new List<MovieStar>()
                        {
                                new MovieStar
                                {
                                    Surename = "Sean",
                                    Name = "Bean",
                                    Job = MovieJob.Actor
                                },
                                new MovieStar
                                {
                                    Surename = "Maria",
                                    Name = "Bello",
                                    Job = MovieJob.Actor
                                },
                                new MovieStar
                                {
                                    Surename = "John",
                                    Name = "Fawcett",
                                    Job = MovieJob.Director
                                },
                                new MovieStar
                                {
                                    Surename = "Stephen",
                                    Name = "Massicotte",
                                    Job = MovieJob.Writer
                                },
                                new MovieStar
                                {
                                    Surename = "Edmund",
                                    Name = "Butt",
                                    Job = MovieJob.Musician
                                }
                        },
                    Tags = new List<ItemTag>()
                        {
                                tags.Single(x => x.Name == "Horror"),
                                tags.Single(x => x.Name == "Mystery")
                        },
                    Rating = new MovieRating()
                    {
                        Actors = 1,
                        Ambience = 1,
                        Plot = 1,
                        Music = 1,
                        Regie = 1
                    }
                }
            };

        private static List<PenAndPaper> CreatePAPs(List<ItemTag> tags)
            => new()
            {
                new()
                {
                    Title = "T.E.A.R.S",
                    Image = "https://pbs.twimg.com/tweet_video_thumb/DreIoEwWkAA5F-M.jpg",
                    Category = ActivityCategory.PAP,
                    Release = new DateTime(2020, 8, 1),
                    Description = "EMPTY",
                    MDContent = "Content Placeholder",
                    Tags = new List<ItemTag>()
                    {
                        tags.Single(x => x.Name == "Pen and Paper"),
                        tags.Single(x => x.Name == "TEARS")
                    }
                },
                new()
                {
                    Title = "Gammaslayers",
                    Image = "http://fc06.deviantart.net/fs70/i/2015/045/6/6/gammaslayers_glowing_fields_by_gammawhisp-d8hxnzb.jpg",
                    Category = ActivityCategory.PAP,
                    Release = new DateTime(2020, 9, 15),
                    Description = "EMPTY",
                    MDContent = "Content Placeholder",
                    Tags = new List<ItemTag>()
                    {
                        tags.Single(x => x.Name == "Pen and Paper"),
                        tags.Single(x => x.Name == "Gammaslayers")
                    }
                },
                new()
                {
                    Title = "Morriton Manor",
                    Image = "https://pbs.twimg.com/media/D9QxlF1XsAAmvv4.jpg:large",
                    Category = ActivityCategory.PAP,
                    Release = new DateTime(2021, 5, 20),
                    Description = "EMPTY",
                    MDContent = "Content Placeholder",
                    Tags = new List<ItemTag>()
                    {
                        tags.Single(x => x.Name == "Pen and Paper"),
                        tags.Single(x => x.Name == "Morriton Manor")
                    }
                }
            };

        private static List<Script> CreateScripts(List<ItemTag> tags)
            => new()
            {
                new()
                {
                    Title = "ef core context example",
                    Description = "Simple EF Core Context setup with model configuration",
                    Release = new DateTime(2021, 8, 1),
                    Type = ScriptType.CSharp,
                    Category = ActivityCategory.Scripts_Csharp,
                    Code = "public class Startup\n{\npublic void ConfigureServices(IServiceCollection services)\n{\nservices.AddDbContext<Context>(options =>\n{\noptions.UseSqlite($\"Data Source = { Configuration.GetConnectionString(\"Database\") }\");\n});\n}\n}\npublic class Context : DbContext\n{\npublic DbSet<Movie> Movies { get; set; }\n\npublic DataContext(DbContextOptions<Context> options) : base(options) { }\nprotected override void OnModelCreating(ModelBuilder modelBuilder)\n{\nmodelBuilder.ApplyConfiguration(new MovieConfig());\n}\n}\npublic class MovieConfig : IEntityTypeConfiguration<Movie>\n{\npublic void Configure(EntityTypeBuilder<Movie> builder)\n{\nbuilder.ToTable(\"movies\");\nbuilder.HasKey(x => x.Id);\nbuilder.Property(e => e.Id).ValueGeneratedOnAdd();\nbuilder.Property(e => e.Title).IsRequired();\n}\n}\npublic class Movie\n{\npublic int Id { get; set; }\npublic string Title { get; set; }\n}",
                    Tags = new List<ItemTag>()
                    {
                        tags.Single(x => x.Name == "Script"),
                        tags.Single(x => x.Name == "C#"),
                        tags.Single(x => x.Name == "EF Core")
                    }
                }
            };
    }
}
