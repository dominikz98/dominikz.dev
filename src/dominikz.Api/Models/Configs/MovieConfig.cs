using dominikz.Common.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Api.Models.Configs
{
    public class MovieConfig : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("movies");

            builder.Property(e => e.MDContent)
                .IsRequired();

            builder.Property(e => e.KeyWord)
                .IsRequired();

            builder.Property(e => e.Thumbnail)
                .IsRequired();

            builder.Property(e => e.Provider)
                .IsRequired()
                .HasConversion(e => (int)e, e => (MovieProvider)e);

            builder.Property(e => e.Runtime)
                .IsRequired();

            builder.HasOne(e => e.Rating)
                .WithOne(e => e.Movie)
                .HasForeignKey<Movie>(e => e.RatingId);

            builder.HasMany(e => e.Stars)
                .WithMany(e => e.Movies);

            builder.Ignore(e => e.Categories);

            builder.Property(e => e.Publication)
                .IsRequired();

            builder.Property(e => e.Watched)
                .IsRequired();

            builder.Property(e => e.USK)
                .IsRequired()
                .HasConversion(e => (int)e, e => (MovieUSK)e);
        }
    }
}
