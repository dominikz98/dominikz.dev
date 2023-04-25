using dominikz.Domain.Models;
using dominikz.Infrastructure.Provider.Database.Converter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Infrastructure.Provider.Database.Entities;

public class FinnhubShadowConfiguration : IEntityTypeConfiguration<FinnhubShadow>
{
    public void Configure(EntityTypeBuilder<FinnhubShadow> builder)
    {
        builder.ToTable("finnhub_shadows");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Date, x.Symbol }).IsUnique();
        builder.Property(x => x.Quarters).HasConversion<QuarterConverter>();
    }
}