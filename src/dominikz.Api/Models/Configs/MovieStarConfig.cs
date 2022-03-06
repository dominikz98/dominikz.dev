using dominikz.Common.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Api.Models.Configs
{
    public class MovieStarConfig : IEntityTypeConfiguration<MovieStar>
    {
        public void Configure(EntityTypeBuilder<MovieStar> builder)
        {
            builder.ToTable("movie_starts");

            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Name)
                .IsRequired();

            builder.Property(e => e.Surename)
                .IsRequired();

            builder.Property(e => e.Job)
                .IsRequired()
                .HasConversion(e => (int)e, e => (MovieJob)e);

            builder.HasMany(e => e.Movies)
                .WithMany(e => e.Stars);
        }
    }
}
