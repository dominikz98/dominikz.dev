using dominikz.Domain.Enums.Movies;

namespace dominikz.Domain.ViewModels.Movies;

public class CreateStreamTokenVm
{
    public Guid Id { get; set; }
    public StreamTokenPrefix Prefix { get; set; }
}