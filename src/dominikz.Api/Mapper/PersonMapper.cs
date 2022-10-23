using dominikz.api.Models;
using dominikz.kernel.ViewModels;

namespace dominikz.api.Mapper;

public static class PersonMapper
{
    public static IQueryable<PersonVM> MapToVM(this IQueryable<Person> query)
        => query.Select(person => person.MapToVM());

    public static PersonVM MapToVM(this Person person)
        => new ()
        {
            Id = person.Id,
            Name = person.Name,
            Image = person.File!.MapToVM()
        };

    public static IEnumerable<PersonVM> MapToVM(this IEnumerable<MoviesPersonsMapping> query)
        => query.Select(mapping => new PersonVM()
        {
            Id = mapping.Person!.Id,
            Name = mapping.Person!.Name,
            Image = mapping.Person!.File!.MapToVM()
        });
}
