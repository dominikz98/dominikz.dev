using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Api.Models.Configs
{
    public class ItemTagConfig : IEntityTypeConfiguration<ItemTag>
    {
        public void Configure(EntityTypeBuilder<ItemTag> builder)
        {
            builder.ToTable("tags");

            builder.HasKey(e => e.Name);

            builder.HasMany(e => e.Activities)
                .WithMany(e => e.Tags);
        }
    }
}
