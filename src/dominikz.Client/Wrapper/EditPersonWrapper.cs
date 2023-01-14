using dominikz.Domain.Attributes;
using dominikz.Domain.Structs;
using dominikz.Domain.ViewModels;

namespace dominikz.Client.Wrapper;

public class EditPersonWrapper : EditPersonVm
{
    [ListNotEmpty(Max = 1)]
    public List<FileStruct> Image { get; set; } = new();
}