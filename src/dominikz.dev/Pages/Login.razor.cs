using System.Web;
using Blazored.LocalStorage;
using dominikz.dev.Utils;
using dominikz.shared.ViewModels;
using dominikz.shared.ViewModels.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace dominikz.dev.Pages;

public partial class Login
{
    [Inject] protected AuthService? AuthService { get; set; }
    [Inject] protected  NavigationManager? NavManager { get; set; }
    
    private EditContext? _editContext;
    private readonly LoginVm _vm = new();
    
    public const string QueryRedirect = "redirect";
    private string _redirectUrl = "/";
    private bool _loginFailed;
    
    protected override async Task OnInitializedAsync()
    {
        _editContext = new(_vm);
        
        // get redirect by query parameter
        _redirectUrl = HttpUtility.UrlDecode(NavManager!.GetQueryParamByKey(QueryRedirect) ?? _redirectUrl);

        var alreadyLoggedIn = await AuthService!.CheckIsLoggedIn();
        if (alreadyLoggedIn == false)
            return;
        
        // already logged in -> redirect
        NavManager!.NavigateTo(_redirectUrl);
    }

    private async Task OnLoginClicked()
    {
        if (_editContext == null || _editContext.Validate() == false)
            return;
        
        _loginFailed = false;
        var success = await AuthService!.Login(_vm);
        if (success == false)
        {
            _loginFailed = true;
            return;
        }
        
        NavManager!.NavigateTo(_redirectUrl);
    }
}