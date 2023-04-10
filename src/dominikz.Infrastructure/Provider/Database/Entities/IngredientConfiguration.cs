using dominikz.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dominikz.Infrastructure.Provider.Database.Entities;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("ingredients");
        builder.HasKey(x => new { x.RecipeId, x.FoodId });
        builder.Property(x => x.RecipeId).HasConversion(new GuidToStringConverter());
        builder.Property(x => x.FoodId).HasConversion(new GuidToStringConverter());
    }
}