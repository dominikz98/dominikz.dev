using dominikz.Api.Models;
using dominikz.Api.Models.Configs;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api
{
    public class DataContext : DbContext
    {
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ItemTag> Tags { get; set; }
        public DbSet<Blogpost> Blogposts { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieRating> MovieRatings { get; set; }
        public DbSet<MovieStar> MovieStars { get; set; }
        public DbSet<PenAndPaper> PenAndPaper { get; set; }
        public DbSet<Script> Scripts { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ActivityConfig());
            modelBuilder.ApplyConfiguration(new ItemTagConfig());
            modelBuilder.ApplyConfiguration(new BlogpostConfig());
            modelBuilder.ApplyConfiguration(new MovieConfig());
            modelBuilder.ApplyConfiguration(new MovieRatingConfig());
            modelBuilder.ApplyConfiguration(new MovieStarConfig());
            modelBuilder.ApplyConfiguration(new PenAndPaperConfig());
            modelBuilder.ApplyConfiguration(new ScriptConfig());
        }
    }
}
