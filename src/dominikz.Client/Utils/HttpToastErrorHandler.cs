using System.Net;
using dominikz.Client.Components.Toast;
using dominikz.Infrastructure.Clients.Api;

namespace dominikz.Client.Utils;

public class HttpToastErrorHandler : IHttpErrorHandler
{
    private readonly ToastService _toast;

    public HttpToastErrorHandler(ToastService toast)
    {
        _toast = toast;
    }

    public Task Handle(HttpStatusCode? code, string message, CancellationToken cancellationToken)
    {
        var codeFirstChar = ((int)(code ?? HttpStatusCode.InternalServerError)).ToString()[0];
        if (codeFirstChar == '5')
        {
            _toast.Show(message, ToastLevel.Error);
            return Task.CompletedTask;
        }
            
        _toast.Show(message, ToastLevel.Warning);
        return Task.CompletedTask;
    }
}