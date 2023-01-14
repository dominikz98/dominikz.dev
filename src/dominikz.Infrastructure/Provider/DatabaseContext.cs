using dominikz.Domain.Models;
using dominikz.Domain.Structs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dominikz.Infrastructure.Provider;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public IQueryable<T> From<T>() where T : class
        => Set<T>().AsQueryable();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var account = builder.Entity<Account>();
        account.ToTable("accounts");
        account.HasKey(x => x.Id);
        
        var article = builder.Entity<Article>();
        article.ToTable("articles");
        article.HasKey(x => x.Id);
        article.Property(x => x.Tags).HasConversion<TagsConverter>(new ListComparer<string>());
        article.HasOne(x => x.File)
            .WithOne(x => x.Article)
            .HasForeignKey<Article>(x => x.Id);

        var movie = builder.Entity<Movie>();
        movie.ToTable("movies");
        
        var book = builder.Entity<Book>();
        book.ToTable("books");

        var game = builder.Entity<Game>();
        game.ToTable("games");

        var media = builder.Entity<Media>();
        media.ToTable("medias");
        media.HasKey(x => x.Id);
        media.HasOne(x => x.File)
            .WithOne(x => x.Media)
            .HasForeignKey<Media>(x => x.Id);

        var moviesPersonsMapping = builder.Entity<MoviesPersonsMapping>();
        moviesPersonsMapping.ToTable("movies_persons_mapping");
        moviesPersonsMapping.HasKey(x => new { x.MovieId, x.PersonId, x.Category });

        var person = builder.Entity<Person>();
        person.ToTable("persons");
        person.HasKey(x => x.Id);
        person.HasOne(x => x.File)
            .WithOne(x => x.Person)
            .HasForeignKey<Person>(x => x.Id);

        var file = builder.Entity<StorageFile>();
        file.ToTable("files");
        file.HasKey(x => x.Id);
    }
}

class TagsConverter : ValueConverter<List<string>, string>
{
    public TagsConverter()
        : base(x => string.Join(';', x.Select(y => y.ToLower().Trim())),
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