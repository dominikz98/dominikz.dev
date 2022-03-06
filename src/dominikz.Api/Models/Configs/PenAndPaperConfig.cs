using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Api.Models.Configs
{
    public class PenAndPaperConfig : IEntityTypeConfiguration<PenAndPaper>
    {
        public void Configure(EntityTypeBuilder<PenAndPaper> builder)
        {
            builder.ToTable("penandpaper");

            builder.Property(e => e.MDContent)
                .IsRequired();

            builder.Property(e => e.Image)
                .IsRequired();
        }
    }
}
