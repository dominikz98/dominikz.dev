using dominikz.dev.Components.Files;
using dominikz.shared.Attributes;
using dominikz.shared.ViewModels.Media;

namespace dominikz.dev.Models;

public class EditMovieWrapper : EditMovieVm
{
    [ListNotEmpty(Max = 1)]
    public List<FileStruct> Image { get; set; } = new();
    
    public List<EditPersonWrapper> DirectorsWrappers { get; set; } = new();
    public List<EditPersonWrapper> WritersWrappers { get; set; } = new();
    public List<EditPersonWrapper> StarsWrappers { get; set; } = new();
}