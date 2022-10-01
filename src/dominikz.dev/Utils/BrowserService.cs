using Microsoft.JSInterop;

namespace dominikz.dev.Utils;

public class BrowserService
{
    private readonly IJSRuntime _jsRuntime;

    public BrowserService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<WindowDimension> GetWindow()
        => await _jsRuntime.InvokeAsync<WindowDimension>("getDimensions");
}

public class WindowDimension
{
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsMobile { get => Width <= 600; }
}