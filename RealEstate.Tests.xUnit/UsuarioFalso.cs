using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace RealEstate.Tests.xUnit
{
    public class UsuarioFalso : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {   new Claim(ClaimTypes.Email, "juancorrear08102@gmail.com"),
                new Claim(ClaimTypes.Name, "JuanCorrea"),
                new Claim(ClaimTypes.NameIdentifier, "31006220-7fa5-41e9-88cc-8b9006bee14a"),
            },"prueba"));
            await next();
        }
    }
}
