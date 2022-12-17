using dominikz.api.Models;
using dominikz.shared.ViewModels;

namespace dominikz.api.Mapper;

public static class PersonMapper
{
    public static IQueryable<PersonVM> MapToVm(this IQueryable<Person> query)
        => query.Select(person => person.MapToVm());

    public static PersonVM MapToVm(this Person person)
        => new ()
        {
            Id = person.Id,
            Name = person.Name,
            Image = person.File!.MapToVm()
        };

    public static IEnumerable<PersonVM> MapToVm(this IEnumerable<MoviesPersonsMapping> query)
        => query.Select(mapping => new PersonVM()
        {
            Id = mapping.Person!.Id,
            Name = mapping.Person!.Name,
            Image = mapping.Person!.File!.MapToVm()
        });
}
