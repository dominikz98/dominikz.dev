using dominikz.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Api.Endpoints;

[ApiKey]
[ApiController]
// [EnableRateLimiting(Policies.RateLimit)]
public class EndpointController : ControllerBase
{
}