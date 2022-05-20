using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Example.Api.Controllers
{
    /// <summary>
    /// The api controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public abstract class ApiController : ControllerBase
    {
    }
}
