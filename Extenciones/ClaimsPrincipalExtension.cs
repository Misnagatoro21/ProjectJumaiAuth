using System.Security.Claims;

namespace ProjectJumaiAuth.Extenciones
{
    public static class ClaimsPrincipalExtensions
    {
        public const string PermissionClaimType = "Permiso";

        public static bool TienePermiso(this ClaimsPrincipal user, string permiso)
        {
            return user.Claims.Any(c =>
                c.Type == PermissionClaimType &&
                string.Equals(c.Value, permiso, StringComparison.OrdinalIgnoreCase));
        }

        public static bool TieneAlgunPermiso(this ClaimsPrincipal user, params string[] permisos)
        {
            return permisos.Any(p => user.TienePermiso(p));
        }

        public static IReadOnlyList<string> GetPermisos(this ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == PermissionClaimType)
                .Select(c => c.Value)
                .OrderBy(p => p)
                .ToList();
        }
    }
}