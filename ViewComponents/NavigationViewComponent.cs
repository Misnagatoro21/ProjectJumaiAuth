using Microsoft.AspNetCore.Mvc;
using ProjectJumaiAuth.Extensions;
using ProjectJumaiAuth.Mejoras;
using ProjectJumaiAuth.Repositories;
using System.Security.Claims;

public class NavigationViewComponent(IUsuarioRepository _userRepository) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = UserClaimsPrincipal;
        IReadOnlyList<string> permission = user?.ObtenerPermisos() ?? [];

        if (user?.Identity?.IsAuthenticated == true &&
            int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id))
        {
            var dbUser = await _userRepository.GetUsuarioByIdWithPermissionAsync(id);
            if (dbUser != null)
                permission = EffectivePermission.FromUser(dbUser);
        }

        return View(permission);
    }
}