using dominikz.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Infrastructure.Provider.Database.Entities;

public class EarningCallConfiguration : IEntityTypeConfiguration<EarningCall>
{
    public void Configure(EntityTypeBuilder<EarningCall> builder)
    {
        builder.ToTable("earnings_calls");
        builder.HasKey(x => x.Id);
    }
}
