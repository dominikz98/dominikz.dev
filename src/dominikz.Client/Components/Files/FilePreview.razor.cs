using dominikz.Domain.Structs;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Files;

public partial class FilePreview
{
    [Parameter] public FileStruct File { get; set; }
    [Parameter] public EventCallback<FileStruct> FileClicked { get; set; }
    [Parameter] public EventCallback<FileStruct> RemoveClicked { get; set; }
}