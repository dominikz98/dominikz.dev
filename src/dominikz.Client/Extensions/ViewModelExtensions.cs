using dominikz.Client.Wrapper;
using dominikz.Domain.ViewModels;

namespace dominikz.Client.Extensions;

public static class ViewModelExtensions
{
    public static EditPersonWrapper Wrap(this EditPersonVm vm)
        => new()
        {
            Id = vm.Id,
            Tracked = vm.Tracked,
            Category = vm.Category,
            Name = vm.Name
        };
}