using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Api.Models.Configs
{
    public class BlogpostConfig : IEntityTypeConfiguration<Blogpost>
    {
        public void Configure(EntityTypeBuilder<Blogpost> builder)
        {
            builder.ToTable("blogposts");

            builder.Property(e => e.MDContent)
                .IsRequired();

            builder.Property(e => e.Banner)
                .IsRequired();
        }
    }
}
