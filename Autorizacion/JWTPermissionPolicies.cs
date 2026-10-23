using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ProyectJumaiAuth.Extenciones;
using System.Security.Claims;

namespace ProyectJumaiAuth.Extenciones
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool TienePermiso(this ClaimsPrincipal user, string permiso)
            => user.HasClaim("permiso", permiso);
    }

    public class JWTPermisionPolicies
    {
        public class permissionRequirement(string permiso) : IAuthorizationRequirement
        {
            public string Permiso { get; } = permiso;
        }

        public class permissionAuthorizationHandler : AuthorizationHandler<permissionRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, permissionRequirement requirement)
            {
                if (context.User.TienePermiso(requirement.Permiso))
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }

            public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
            {
                public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
                {
                    var policy = await base.GetPolicyAsync(policyName);
                    if (policy is not null)
                        return policy;
                    return new AuthorizationPolicyBuilder()
                        .AddRequirements(new permissionRequirement(policyName))
                        .Build();
                }
            }
        }
    }
}
