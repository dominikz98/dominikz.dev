using dominikz.api.Models;
using dominikz.shared.ViewModels.Media;

namespace dominikz.api.Mapper;

public static class BookMapper
{
    public static IQueryable<BookVm> MapToVm(this IQueryable<Book> query)
        => query.Select(book => new BookVm()
        {
            Id = book.Id,
            Title = book.Title,
            PublishDate = book.PublishDate,
            ImageUrl = book.File!.Id.ToString(),
            Genres = book.Genres,
            Language = book.Language,
            Year = book.Year,
            Author = book.Author
        });
}
