namespace dominikz.Domain.Enums;

[Flags]
public enum PersonCategoryFlags
{
    Creator = 0,
    Author = 2,
    Director = 4,
    Writer = 8,
    Star = 16
}
