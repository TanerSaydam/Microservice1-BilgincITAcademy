using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.ProductWebAPI.Controllers;

[ApiController]
[Route("[Controller]/getall")]
public sealed class ProductsController : ControllerBase
{
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [MyAuthorize("product.getall")]
    public IActionResult GetAll()
    {
        return Ok();
    }
}
