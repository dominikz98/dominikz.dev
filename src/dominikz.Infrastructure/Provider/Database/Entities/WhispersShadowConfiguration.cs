using dominikz.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Infrastructure.Provider.Database.Entities;

public class WhispersShadowConfiguration : IEntityTypeConfiguration<WhispersShadow>
{
    public void Configure(EntityTypeBuilder<WhispersShadow> builder)
    {
        builder.ToTable("whispers_shadows");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Date, x.Symbol });
    }
}