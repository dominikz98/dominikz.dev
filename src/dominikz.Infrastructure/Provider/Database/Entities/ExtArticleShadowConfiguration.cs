using dominikz.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Infrastructure.Provider.Database.Entities;

public class ExtArticleShadowConfiguration : IEntityTypeConfiguration<ExtArticleShadow>
{
    public void Configure(EntityTypeBuilder<ExtArticleShadow> builder)
    {
        builder.ToTable("ext_article_shadows");
        builder.HasKey(x => new { x.Title, x.Date });
        builder.Ignore(x => x.Image);
    }
}