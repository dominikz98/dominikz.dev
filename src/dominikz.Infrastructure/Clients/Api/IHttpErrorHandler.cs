using System.Net;

namespace dominikz.Infrastructure.Clients.Api;

public interface IHttpErrorHandler
{
    Task Handle(HttpStatusCode? code, string message, CancellationToken cancellationToken);
}