using dominikz.Client.Wrapper;
using dominikz.Domain.Enums;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components;

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