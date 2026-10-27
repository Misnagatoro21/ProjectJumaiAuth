using System.Security.Claims;

namespace ProjectJumaiAuth.Authorization
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool TienePermiso(this ClaimsPrincipal user, string permiso)
            => user.HasClaim("permission", permiso);
    }
}