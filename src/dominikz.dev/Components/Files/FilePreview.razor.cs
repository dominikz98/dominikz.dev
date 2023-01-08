using dominikz.dev.Utils;
using dominikz.shared.Contracts;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components.Files;

public partial class FilePreview
{
    [Parameter] public FileStruct File { get; set; }
    [Parameter] public EventCallback<FileStruct> FileClicked { get; set; }
    [Parameter] public EventCallback<FileStruct> RemoveClicked { get; set; }
}