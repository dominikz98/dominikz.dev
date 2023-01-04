using dominikz.api.Attributes;
using dominikz.api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace dominikz.api.Endpoints;

[ApiKey]
[ApiController]
[EnableRateLimiting(Policies.RateLimit)]
public class EndpointController : ControllerBase
{
}