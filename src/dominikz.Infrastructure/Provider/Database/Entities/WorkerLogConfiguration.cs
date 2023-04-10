using dominikz.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Infrastructure.Provider.Database.Entities;

public class WorkerLogConfiguration : IEntityTypeConfiguration<WorkerLog>
{
    public void Configure(EntityTypeBuilder<WorkerLog> builder)
    {
        builder.ToTable("worker_log");
        builder.HasKey(x => x.Id);
    }
}