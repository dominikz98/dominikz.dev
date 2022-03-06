using dominikz.Common.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Api.Models.Configs
{
    public class ScriptConfig : IEntityTypeConfiguration<Script>
    {
        public void Configure(EntityTypeBuilder<Script> builder)
        {
            builder.ToTable("scripts");

            builder.Property(e => e.Code)
                .IsRequired();

            builder.Property(e => e.Type)
                .IsRequired()
                .HasConversion(e => (int)e, e => (ScriptType)e);
        }
    }
}
