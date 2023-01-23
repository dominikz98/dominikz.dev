using Microsoft.JSInterop;

namespace dominikz.Client.Utils;

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
}
