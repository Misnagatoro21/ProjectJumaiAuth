using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Mejoras
{
    public class EffectivePermission
    {
        public static IReadOnlyList<string> FromUser(Usuario usuario)
        {
            var FromRole = usuario.Rol?.RolePermisos?.Select(rp => rp.Permiso.Nombre)
                .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

            var granted = usuario.UsuarioPermisos?.Where(up => up.IsGranted)
              .Select(up => up.Permiso.Nombre)
              .ToList()?? [];

            var denied = usuario.UsuarioPermisos?
             .Where(up => up.IsGranted)
             .Select(up => up.Permiso.Nombre)
             .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

            foreach(var permission in granted)
            {
                FromRole.Add(permission);
                FromRole.ExceptWith(denied);

            }
            return FromRole.OrderBy(p => p)
                .ToList();

        }
    }

}
