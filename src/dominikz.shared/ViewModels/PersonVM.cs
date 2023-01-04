using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public class PersonVM : IViewModel, IHasImageUrl
{
    public Guid Id { get; init; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
