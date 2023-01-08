using System.Text.Json;
using dominikz.dev.Endpoints;
using dominikz.dev.Models;
using dominikz.shared.Enums;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Components;

public partial class Person
{
    [Parameter] public EditPersonWrapper Data { get; set; } = new();
    [Inject] public DownloadEndpoints? DownloadEndpoints { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Data.Image.Count > 0 || Data.Tracked == false)
            return;

        var file = await DownloadEndpoints!.Image(Data.Id, true, ImageSizeEnum.Original);
        if (file == null)
            return;

        Data.Image.Add(file.Value);
    }
}