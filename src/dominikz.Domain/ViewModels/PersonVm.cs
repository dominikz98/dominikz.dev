using System.ComponentModel.DataAnnotations;
using dominikz.Domain.Attributes;
using dominikz.Domain.Contracts;
using dominikz.Domain.Enums;

namespace dominikz.Domain.ViewModels;

public class EditPersonVm
{
    public Guid Id { get; set; }
    public bool Tracked { get; set; }

    [RequiredEnum<PersonCategoryFlags>(Blacklist = new[] { PersonCategoryFlags.Creator })]
    public PersonCategoryFlags Category { get; set; }

    [MinLength(5)] public string Name { get; set; } = string.Empty;
}

public class PersonVm : IHasImageUrl
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}