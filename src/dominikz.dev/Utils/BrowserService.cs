using Microsoft.JSInterop;

namespace dominikz.dev.Utils;

public class BrowserService
{
    private readonly IJSRuntime _jsRuntime;

    public BrowserService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task CopyToClipboard(string text)
        => await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);

    public async Task ChangeCarouselScrollLeft(bool add)
        => await _jsRuntime.InvokeVoidAsync("changeCarouselScrollLeft", add);

    public async Task PlayAudio(string id, string path)
        => await _jsRuntime.InvokeVoidAsync("setAudioSource", id, path);

    public async Task StopAudio(string id)
        => await _jsRuntime.InvokeVoidAsync("stopAudio", id);
}

public class WindowDimension
{
    public int Width { get; set; }
    public int Height { get; set; }

    public bool IsMobile
    {
        get => Width <= 600;
    }
}