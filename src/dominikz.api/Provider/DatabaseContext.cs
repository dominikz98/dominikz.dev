using dominikz.api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dominikz.api.Provider;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public IQueryable<T> From<T>() where T : class
        => Set<T>().AsQueryable();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var article = builder.Entity<Article>();
        article.ToTable("articles");
        article.HasKey(x => x.Id);
        article.Property(x => x.Tags).HasConversion<TagsConverter>(new ListComparer<string>());

        var book = builder.Entity<Book>();
        book.ToTable("books");

        var game = builder.Entity<Game>();
        game.ToTable("games");

        var media = builder.Entity<Media>();
        media.ToTable("medias");
        media.HasKey(x => x.Id);

        var movie = builder.Entity<Movie>();
        movie.ToTable("movies");

        var moviesPersonsMapping = builder.Entity<MoviesPersonsMapping>();
        moviesPersonsMapping.ToTable("movies_persons_mapping");
        moviesPersonsMapping.HasKey(x => new { x.MovieId, x.PersonId });

        var author = builder.Entity<Person>();
        author.ToTable("persons");
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