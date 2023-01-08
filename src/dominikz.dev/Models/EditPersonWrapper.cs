using dominikz.dev.Components.Files;
using dominikz.shared.Attributes;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Models;

public class EditPersonWrapper : EditPersonVm
{
    [ListNotEmpty(Max = 1)]
    public List<FileStruct> Image { get; set; } = new();
}