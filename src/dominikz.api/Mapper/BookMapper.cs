using dominikz.api.Models;
using dominikz.shared.ViewModels.Media;

namespace dominikz.api.Mapper;

public static class BookMapper
{
    public static IQueryable<BookVM> MapToVm(this IQueryable<Book> query)
        => query.Select(book => new BookVM()
        {
            Id = book.Id,
            Title = book.Title,
            Timestamp = book.Timestamp,
            Image = book.File!.MapToVm(),
            Genres = book.Genres,
            Language = book.Language,
            Year = book.Year,
            Author = book.Author
        });
}
