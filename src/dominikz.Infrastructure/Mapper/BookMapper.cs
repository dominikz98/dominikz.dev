using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Media;

namespace dominikz.Infrastructure.Mapper;

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
