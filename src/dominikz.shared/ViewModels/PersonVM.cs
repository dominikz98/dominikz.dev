using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public class PersonVM : IViewModel
{
    public Guid Id { get; init; }
    public FileVM? Image { get; init; }
    public string Name { get; init; } = string.Empty;
}
