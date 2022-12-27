using System.Web;
using dominikz.dev.Pages;
using Microsoft.AspNetCore.Components;

namespace dominikz.dev.Shared;

public partial class AppBar
{
    [Parameter] public EventCallback OnExpandClicked { get; set; }
    [Inject] protected NavigationManager? NavigationManager { get; set; }

    private async Task CallOnExpandClicked()
        => await OnExpandClicked.InvokeAsync();

    private void OnLoginClicked()
    {
        var redirect = NavigationManager!.ToAbsoluteUri(NavigationManager.Uri).PathAndQuery;
        if (redirect.StartsWith('/'))
            redirect = redirect.Remove(0, 1);
        
        NavigationManager!.NavigateTo($"/login?{Login.QueryRedirect}={HttpUtility.UrlEncode(redirect)}");
    }
}