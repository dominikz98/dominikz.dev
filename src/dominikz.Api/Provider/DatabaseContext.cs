using dominikz.api.Models;
using dominikz.kernel.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dominikz.api.Provider;

public class DatabaseContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Media> Medias { get; set; }
    public DbSet<StorageFile> Files { get; set; }
    public DbSet<Food> Foods { get; set; }
    public DbSet<Recipe> Recipes { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var article = builder.Entity<Article>();
        article.ToTable("articles");
        article.HasKey(x => x.Id);
        article.Property(x => x.Tags).HasConversion<TagsConverter>(new ListComparer<string>());

        var media = builder.Entity<Media>();
        media.ToTable("medias");
        media.HasKey(x => x.Id);

        var author = builder.Entity<Author>();
        author.ToTable("authors");
        author.HasKey(x => x.Id);

        var recipesFoodsMapping = builder.Entity<RecipesFoodsMapping>();
        recipesFoodsMapping.ToTable("recipes_foods_mapping");
        recipesFoodsMapping.HasKey(x => new { x.RecipeId, x.FoodId });

        var foods = builder.Entity<Food>();
        foods.ToTable("foods");
        foods.HasKey(x => x.Id);

        var recipes = builder.Entity<Recipe>();
        recipes.ToTable("recipes");
        recipes.HasKey(x => x.Id);

        var file = builder.Entity<StorageFile>();
        file.ToTable("files");
        file.HasKey(x => x.Id);
    }
}

class TagsConverter : ValueConverter<List<string>, string>
{
    public TagsConverter()
        : base(x => string.Join(';', x.Select(x => x.ToLower().Trim())),
            x => x.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
    { }
}

class ListComparer<T> : ValueComparer<List<T>>
{
    public ListComparer()
        : base((c1, c2) => (c1 ?? new()).SequenceEqual(c2 ?? new()),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())),
            c => c.ToList())
    { }
}