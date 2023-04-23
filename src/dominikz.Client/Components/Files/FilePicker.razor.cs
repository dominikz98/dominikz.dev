using System.ComponentModel;
using dominikz.Domain.Enums.Files;
using dominikz.Domain.Structs;
using dominikz.Infrastructure.Utils;
using HeyRed.Mime;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace dominikz.Client.Components.Files;

public partial class FilePicker
{
    [Parameter] public long MaxAllowedSize { get; set; } = 1024 * 1024 * 10;
    [Parameter] public FileCategoryEnum[] Allowed { get; set; } = new FileCategoryEnum[] { FileCategoryEnum.Image };
    [Parameter] public bool HideUploadIfCompleted { get; set; }

    [Parameter] public int MaxSelectedFiles { get; set; } = 1;
    [Parameter] public List<FileStruct> Selected { get; set; } = new();
    [Parameter] public EventCallback<List<FileStruct>> SelectedChanged { get; set; }

    [Parameter] public int MaxFiles { get; set; } = 10;
    [Parameter] public List<FileStruct> Files { get; set; } = new();
    [Parameter] public EventCallback<List<FileStruct>> FilesChanged { get; set; }

    private string? _error;

    private async Task CallFileSelected(FileStruct file)
    {
        Selected.Insert(0, file);
        Files.Remove(file);

        if (Selected.Count > MaxSelectedFiles)
        {
            var deselect = Selected.Last();
            Selected.Remove(deselect);
            Files.Insert(0, deselect);
            Files = Files.Take(MaxFiles).ToList();
        }

        await SelectedChanged.InvokeAsync(Selected);
        await FilesChanged.InvokeAsync(Files);
    }

    private async Task CallFileDeselected(FileStruct file)
    {
        if (Selected.Count == 1)
            return;

        Files.Insert(0, file);
        Selected.Remove(file);

        await SelectedChanged.InvokeAsync(Selected);
        await FilesChanged.InvokeAsync(Files);
    }

    private async Task CallSelectedFileRemoved(FileStruct file)
    {
        Selected.Remove(file);

        if (Selected.Count > 0)
        {
            await SelectedChanged.InvokeAsync(Selected);
            return;
        }

        var toSelect = Files.OrderBy(x => x.Name).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(toSelect.Name))
            return;

        await CallFileSelected(toSelect);
    }

    private async Task FileRemoved(FileStruct file)
    {
        Files.Remove(file);
        await FilesChanged.InvokeAsync(Files);
    }

    private async Task LoadFiles(InputFileChangeEventArgs e)
    {
        _error = null;

        foreach (var file in e.GetMultipleFiles(MaxFiles))
        {
            var result = await ParseToFile(file);
            if (result == null)
                continue;

            Files.Insert(0, result.Value);
        }

        Files = Files.Take(MaxFiles).ToList();
        if (Selected.Count > 0)
        {
            await FilesChanged.InvokeAsync(Files);
            return;
        }

        var toSelect = Files.OrderBy(x => x.Name).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(toSelect.Name) == false)
        {
            await CallFileSelected(toSelect);
            return;
        }

        await SelectedChanged.InvokeAsync(Selected);
        await FilesChanged.InvokeAsync(Files);
    }

    private async Task<FileStruct?> ParseToFile(IBrowserFile? file)
    {
        if (file is null)
            return null;

        // catch stream
        var stream = file.OpenReadStream(MaxAllowedSize);
        var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ms.Position = 0;

        var contentType = MimeTypesMap.GetMimeType(file.Name);
        var parsed = new FileStruct(file.Name, contentType, ms);
        var category = FileIdentifier.GetCategoryByName(parsed.Name);
        if (Allowed.Contains(category)) 
            return parsed;
        
        _error = "Invalid!";
        throw new WarningException();
    }
}