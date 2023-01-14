using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Toast;

public partial class Toast : IDisposable
{
    [Inject] protected ToastService? Service { get; set; }

    private string _heading = string.Empty;
    private string _message = string.Empty;
    private bool _isVisible;
    private string _backgroundCssClasses = string.Empty;
    private string _iconCssClasses = string.Empty;

    protected override void OnInitialized()
    {
        Service!.OnShow += ShowToast;
        Service!.OnHide += HideToast;
    }

    private void ShowToast(string message, ToastLevel level)
    {
        BuildToastSettings(level, message);
        _isVisible = true;
        StateHasChanged();
    }

    private void HideToast()
    {
        _isVisible = false;
        StateHasChanged();
    }

    private void BuildToastSettings(ToastLevel level, string message)
    {
        switch (level)
        {
            case ToastLevel.Info:
                _backgroundCssClasses = "bg-info";
                _iconCssClasses = "info";
                _heading = "Info";
                break;
            case ToastLevel.Success:
                _backgroundCssClasses = "bg-success";
                _iconCssClasses = "check";
                _heading = "Success";
                break;
            case ToastLevel.Warning:
                _backgroundCssClasses = "bg-warning";
                _iconCssClasses = "exclamation";
                _heading = "Warning";
                break;
            case ToastLevel.Error:
                _backgroundCssClasses = "bg-danger";
                _iconCssClasses = "times";
                _heading = "Error";
                break;
        }

        _message = message;
    }

    public void Dispose()
        => Service!.OnShow -= ShowToast;
}