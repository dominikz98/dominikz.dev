using System.Net;

namespace dominikz.Client.Api;

public interface IHttpErrorHandler
{
    Task Handle(HttpStatusCode? code, string message, CancellationToken cancellationToken);
}