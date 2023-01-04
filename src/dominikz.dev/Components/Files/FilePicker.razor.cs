using System.ComponentModel;
using dominikz.dev.Utils;
using dominikz.shared;
using dominikz.shared.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace dominikz.dev.Components.Files;

public partial class FilePicker
{
    [Parameter] public int MaxAllowedFiles { get; set; } = 10;
    [Parameter] public long MaxAllowedSize { get; set; } = 1024 * 1024 * 10;
    [Parameter] public FileCategoryEnum[] Allowed { get; set; } = new FileCategoryEnum[] { FileCategoryEnum.Image };
    [Parameter] public List<FileStruct> Files { get; set; } = new();
    [Parameter] public EventCallback<List<FileStruct>> FilesChanged { get; set; }
    
    // private FilePreview Ref
    // {
    //     set
    //     {
    //         _previews.Add(value);
    //         value.
    //     }
    // }
    //
    // private List<FilePreview> _previews = new();

    private string? _error;

    private async Task LoadFiles(InputFileChangeEventArgs e)
    {
        _error = null;

        foreach (var file in e.GetMultipleFiles(MaxAllowedFiles))
        {
            var extension = FileIdentifier.GetExtensionByName(file.Name);
            var category = FileIdentifier.GetCategoryByExtension(extension);
            if (Allowed.Contains(category) == false)
            {
                _error = "Invalid file!";
                throw new WarningException();
            }

            // catch stream
            var stream = file.OpenReadStream(MaxAllowedSize);
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            ms.Position = 0;

            Files.Insert(0, new(file.Name, ms));
            ms.Position = 0;
        }

        Files = Files.Take(MaxAllowedFiles).ToList();
        await FilesChanged.InvokeAsync(Files);
    }
}