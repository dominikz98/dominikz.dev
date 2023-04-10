using dominikz.Domain.Models;
using dominikz.Infrastructure.Provider.Database.Converter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dominikz.Infrastructure.Provider.Database.Entities;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("articles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(new GuidToStringConverter());
        builder.Property(x => x.Tags).HasConversion<TagsConverter>(new ListComparer<string>());
    }
}