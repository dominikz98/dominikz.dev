using dominikz.api.Models;
using dominikz.shared.Enums;
using dominikz.shared.ViewModels;

namespace dominikz.api.Mapper;

public static class PersonMapper
{
    public static PersonVm MapToVm(this Person person)
        => new ()
        {
            Id = person.Id,
            Name = person.Name,
            ImageUrl = person.File!.Id.ToString()
        };

    public static IEnumerable<PersonVm> MapToVm(this IEnumerable<MoviesPersonsMapping> query)
        => query.Select(mapping => new PersonVm()
        {
            Id = mapping.Person!.Id,
            Name = mapping.Person!.Name,
            ImageUrl = mapping.Person!.File!.Id.ToString()
        });
    
    public static IEnumerable<EditPersonVm> MapToEditVm(this IEnumerable<MoviesPersonsMapping> query)
        => query.Select(mapping => new EditPersonVm()
        {
            Id = mapping.Person!.Id,
            Name = mapping.Person!.Name,
            Tracked = true,
            Category = mapping.Category
        });
}
