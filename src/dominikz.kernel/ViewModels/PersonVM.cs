using dominikz.kernel.Contracts;

namespace dominikz.kernel.ViewModels;

public class PersonVM : IViewModel
{
    public Guid Id { get; set; }
    public FileVM? Image { get; set; }
    public string Name { get; set; } = string.Empty;
}
