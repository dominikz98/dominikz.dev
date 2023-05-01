using System.Web;
using dominikz.Client.Api;
using dominikz.Client.Pages;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client;

public partial class AppBar
{
    [Parameter] public EventCallback OnExpandClicked { get; set; }
    [Inject] protected NavigationManager? NavigationManager { get; set; }
    [Inject] protected ICredentialStorage? Credentials { get; set; }

    private bool _isLoggedIn;
    private bool _streamingMode;

    protected override async Task OnInitializedAsync()
    {
        _isLoggedIn = await Credentials!.IsLoggedIn();
        _streamingMode = await Credentials!.IsStreamingModeEnabled();
    }

    private async Task CallOnExpandClicked()
        => await OnExpandClicked.InvokeAsync();

    private void OnLoginClicked()
    {
        var redirect = NavigationManager!.ToAbsoluteUri(NavigationManager.Uri).PathAndQuery;
        if (redirect.StartsWith('/'))
            redirect = redirect.Remove(0, 1);

        NavigationManager!.NavigateTo($"/login?{Login.QueryRedirect}={HttpUtility.UrlEncode(redirect)}");
    }

    private async Task OnLogoutClicked()
    {
        await Credentials!.Clear();
        _isLoggedIn = false;
    }
}