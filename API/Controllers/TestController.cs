using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("Get")]
    public ActionResult Get()
    {
        return Ok("Hello World!");
    }
    
    [Authorize]
    [HttpGet("GetAuthorised")]
    public ActionResult GetAuthorised()
    {
        return Ok("Hello World!");
    }
}