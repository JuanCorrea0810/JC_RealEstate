using Microsoft.AspNetCore.Authorization;

namespace RealEstate.Tests.xUnit
{
    public class AuthorizationRequirements : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            //Saltarme todos los requerimientos de seguridad como filtros de autorización
            foreach (var requirement in context.PendingRequirements.ToList())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
