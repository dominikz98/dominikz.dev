using dominikz.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dominikz.Infrastructure.Provider.Database.Entities;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("movies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(new GuidToStringConverter());
    }
}