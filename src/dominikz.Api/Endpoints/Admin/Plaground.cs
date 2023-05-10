using dominikz.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace dominikz.Api.Endpoints.Admin;

[Tags("admin")]
[ApiKey(RequiresMasterKey = true)]
[ApiController]
[Route("api/admin/playground")]
public class Playground : ControllerBase
{
  
}