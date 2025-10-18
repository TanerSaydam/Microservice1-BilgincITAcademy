using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Microservice.ProductWebAPI;

public sealed class MyAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _role;
    public MyAuthorizeAttribute(string role)
    {
        _role = role;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string? userId = context.HttpContext.User.FindFirstValue("UserId");

        //db ye bağlan rolü kontrolü

        bool isHaveRole = false;
        if (!isHaveRole)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
