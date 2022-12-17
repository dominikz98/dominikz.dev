using dominikz.shared.Contracts;

namespace dominikz.shared.ViewModels;

public class PersonVM : IViewModel
{
    public Guid Id { get; set; }
    public FileVM? Image { get; set; }
    public string Name { get; set; } = string.Empty;
}
