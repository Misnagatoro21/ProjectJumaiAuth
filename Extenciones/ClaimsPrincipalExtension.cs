using System.Security.Claims;

namespace ProjectJumaiAuth.Extenciones
{
    public class ClaimsPrincipalExtension
    {
        public const string PermissionClaimType = "Permiso";



        public static bool TienePermiso(this ClaimsPrincipal user, string permiso)
        {
            return user.Claims.Any(c => c.Type == PermissionClaimType &&
            string.Equals(c.Value, permiso, StringComparison.OrdinalIgnoreCase));
        }


        public static bool TienePermiso(this ClaimsPrincipal user, params string[] permiso)
        {
            return permiso.Any(user.TienePermiso);
        }


        public static IReadOnlyList<string> Getpermiso(this ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == PermissionClaimType)
                .Select(c => c.Value)
                .OrderBy(p => p)
                .ToList();
        }
}
}
