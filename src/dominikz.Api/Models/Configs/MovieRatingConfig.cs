using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Api.Models.Configs
{
    public class MovieRatingConfig : IEntityTypeConfiguration<MovieRating>
    {
        public void Configure(EntityTypeBuilder<MovieRating> builder)
        {
            builder.ToTable("movie_ratings");

            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Actors)
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(e => e.Ambience)
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(e => e.Music)
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(e => e.Plot)
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(e => e.Regie)
                .HasMaxLength(5)
                .IsRequired();

            builder.HasOne(e => e.Movie)
                .WithOne(e => e.Rating);
        }
    }
}
