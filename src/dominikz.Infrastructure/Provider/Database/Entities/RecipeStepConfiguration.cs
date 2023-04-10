using dominikz.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dominikz.Infrastructure.Provider.Database.Entities;

public class RecipeStepConfiguration : IEntityTypeConfiguration<RecipeStep>
{
    public void Configure(EntityTypeBuilder<RecipeStep> builder)
    {
        builder.ToTable("recipe_steps");
        builder.HasKey(x => new { x.RecipeId, x.Order });
        builder.Property(x => x.RecipeId).HasConversion(new GuidToStringConverter());
    }
}