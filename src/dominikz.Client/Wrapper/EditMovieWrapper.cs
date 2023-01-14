using dominikz.Domain.Attributes;
using dominikz.Domain.Structs;
using dominikz.Domain.ViewModels.Media;

namespace dominikz.Client.Wrapper;

public class EditMovieWrapper : EditMovieVm
{
    [ListNotEmpty(Max = 1)]
    public List<FileStruct> Image { get; set; } = new();
    
    public List<EditPersonWrapper> DirectorsWrappers { get; set; } = new();
    public List<EditPersonWrapper> WritersWrappers { get; set; } = new();
    public List<EditPersonWrapper> StarsWrappers { get; set; } = new();
}