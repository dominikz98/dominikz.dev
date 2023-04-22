using dominikz.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Infrastructure.Provider.Database.Entities;

public class EarningCallConfiguration : IEntityTypeConfiguration<EarningCall>
{
    public void Configure(EntityTypeBuilder<EarningCall> builder)
    {
        builder.ToTable("earning_calls");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Date, x.Symbol }).IsUnique();
    }
}