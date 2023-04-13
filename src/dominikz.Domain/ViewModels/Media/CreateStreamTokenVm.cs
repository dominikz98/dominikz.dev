using dominikz.Domain.Enums.Media;

namespace dominikz.Domain.ViewModels.Media;

public class CreateStreamTokenVm
{
    public Guid Id { get; set; }
    public StreamTokenPrefix Prefix { get; set; }
}