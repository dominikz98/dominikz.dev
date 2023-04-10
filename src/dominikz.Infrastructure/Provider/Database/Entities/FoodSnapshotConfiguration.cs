using dominikz.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dominikz.Infrastructure.Provider.Database.Entities;

public class FoodSnapshotConfiguration : IEntityTypeConfiguration<FoodSnapshot>
{
    public void Configure(EntityTypeBuilder<FoodSnapshot> builder)
    {
        builder.ToTable("food_snapshots");
        builder.HasKey(x => new { x.FoodId, x.Timestamp });
        builder.Property(x => x.FoodId).HasConversion(new GuidToStringConverter());
    }
}