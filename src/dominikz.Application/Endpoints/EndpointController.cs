using dominikz.Application.Attributes;
using dominikz.Application.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace dominikz.Application.Endpoints;

[ApiKey]
[ApiController]
// [EnableRateLimiting(Policies.RateLimit)]
public class EndpointController : ControllerBase
{
}