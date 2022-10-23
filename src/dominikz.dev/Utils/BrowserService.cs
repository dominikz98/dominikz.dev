using Microsoft.JSInterop;

namespace dominikz.dev.Utils;

public class BrowserService
{
    private readonly IJSRuntime _jsRuntime;

    public BrowserService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    //public async Task<double> GetWidthById(string id)
    //    => await _jsRuntime.InvokeAsync<double>("getWidthById", id);

    public async Task<WindowDimension> GetWindow()
        => await _jsRuntime.InvokeAsync<WindowDimension>("getDimensions");

    public async Task ChangeCarouselScrollLeft(bool add)
        => await _jsRuntime.InvokeVoidAsync("changeCarouselScrollLeft", add);
}

public class WindowDimension
{
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsMobile { get => Width <= 600; }
}