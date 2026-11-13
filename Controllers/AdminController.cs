using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Extensions;
using ProjectJumaiAuth.Mejoras;
using ProjectJumaiAuth.Repositories;
using ProjectJumaiAuth.Services;
using System.Security.Claims;

namespace ProjectJumaiAuth.Controllers
{
    public class AdminController(IUsuarioRepository _userRepository, IRolPermissionServices _rolePermissionService) : Controller
    {
        private async Task<IReadOnlyList<string>> GetEffectivePermissionAsync()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Array.Empty<string>();

            var dbUser = await _userRepository.GetUsuarioByIdWithPermissionAsync(userId);
            return dbUser != null
                ? EffectivePermission.FromUser(dbUser)
                : Array.Empty<string>();
        }

        [Authorize(Policy = "admin.panel")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Permissions = await GetEffectivePermissionAsync();
            return View();
        }

        // ---------------- PERMISOS ----------------

        [Authorize(Policy = "permiso.ver")]
        [HttpPost]
        public async Task<IActionResult> GetPermissionData([FromForm] DataTableRequestDto request)
        {
            var result = await _rolePermissionService.GetPermissionPagedAsync(request);
            return Json(result);
        }

        [Authorize(Policy = "permiso.ver")]
        public async Task<IActionResult> Permissions()
        {
            var effective = await GetEffectivePermissionAsync();

            ViewBag.CanCreate = effective.Contains("permiso.crear", StringComparer.OrdinalIgnoreCase);
            ViewBag.CanUpdate = effective.Any(p =>
                string.Equals(p, "permiso.actualizar", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(p, "permiso.crear", StringComparison.OrdinalIgnoreCase));
            ViewBag.CanDelete = effective.Any(p =>
                string.Equals(p, "permiso.eliminar", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(p, "permiso.crear", StringComparison.OrdinalIgnoreCase));
            return View();
        }

        [Authorize(Policy = "permiso.ver")]
        [HttpGet]
        public IActionResult PermissionDetails(int id)
        {
            return View();
        }

        [Authorize(Policy = "permiso.ver")]
        [HttpGet]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var permission = await _rolePermissionService.GetPermissionByIdAsync(id);
            if (permission == null)
                return NotFound(new { message = "Permiso no encontrado" });
            return Ok(permission);
        }

        [Authorize(Policy = "permiso.crear")]
        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] SavePermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Revise los datos del formulario" });

            var (success, _, errorMessage) = await _rolePermissionService.CreatePermissionAsync(dto);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, message = $"Permiso '{dto.Name}' creado correctamente" });
        }

        [Authorize(Policy = "permiso.crear")]
        [HttpPost]
        public async Task<IActionResult> UpdatePermission([FromBody] SavePermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Revise los datos del formulario" });

            var (success, errorMessage) = await _rolePermissionService.UpdatePermissionAsync(dto);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, message = $"Permiso '{dto.Name}' actualizado correctamente" });
        }

        [Authorize(Policy = "permiso.eliminar")]
        [HttpPost]
        public async Task<IActionResult> DeletePermission([FromBody] SavePermissionDto dto)
        {
            if (!ModelState.IsValid || !dto.Id.HasValue)
                return BadRequest(new { message = "Revise los datos del formulario" });

            var (success, errorMessage) = await _rolePermissionService.DeletePermissionAsync(dto.Id.Value);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, message = $"Permiso '{dto.Name}' eliminado correctamente" });
        }

        // ---------------- ROLES ----------------

        [Authorize(Policy = "rol.ver")]
        public async Task<IActionResult> Roles()
        {
            var roles = await _rolePermissionService.GetAllRolesAsync();
            return View(roles);
        }

        [Authorize(Policy = "rol.crear")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRol(CreateRolDto dto)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _rolePermissionService.GetAllRolesAsync();
                ViewBag.CreateError = "Revise los datos del formulario";
                return View("Roles", roles);
            }

            var (success, _, errorMessage) = await _rolePermissionService.CreateRoleAsync(dto);
            if (!success)
            {
                var roles = await _rolePermissionService.GetAllRolesAsync();
                ViewBag.CreateError = errorMessage;
                return View("Roles", roles);
            }

            TempData["Success"] = $"Rol '{dto.Name}' creado correctamente";
            return RedirectToAction(nameof(Roles));
        }

        [Authorize(Policy = "rol.asignar")]
        public async Task<IActionResult> RoleDetails(int id)
        {
            var role = await _rolePermissionService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound(new { message = "Rol no encontrado" });

            ViewBag.AllPermissions = await _rolePermissionService.GetAllPermissionAsync();
            return View(role);
        }

        [Authorize(Policy = "rol.asignar")]
        [HttpPost]
        public async Task<IActionResult> AssignPermissionToRole(int roleId, int permissionId)
        {
            var (success, errorMessage) = await _rolePermissionService.AssignPermissionsToRoleAsync(roleId, permissionId);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, message = "Permiso asignado al rol correctamente" });
        }

        [Authorize(Policy = "rol.asignar")]
        [HttpPost]
        public async Task<IActionResult> RemovePermissionFromRole(int roleId, int permissionId)
        {
            var (success, errorMessage) = await _rolePermissionService.RemovePermissionFromRoleAsync(roleId, permissionId);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, message = "Permiso removido del rol correctamente" });
        }

        // ---------------- USUARIOS ----------------

        [Authorize(Policy = "usuario.ver")]
        public async Task<IActionResult> Users()
        {
            var users = await _rolePermissionService.GetUserListAsync();
            return View(users);
        }

        [Authorize(Policy = "usuario.asignar")]
        public async Task<IActionResult> UserPermissions(int id)
        {
            var user = await _rolePermissionService.GetUserPermissionAsync(id);
            if (user == null)
                return NotFound();

            ViewBag.AllPermissions = await _rolePermissionService.GetAllPermissionAsync();
            ViewBag.AllRoles = await _rolePermissionService.GetAllRolesAsync();
            ViewBag.CanChangeRole = User.ObtenerPermisos().Contains("usuario.cambiar_rol");
            return View(user);
        }

        [Authorize(Policy = "usuario.asignar")]
        [HttpPost]
        public async Task<IActionResult> AssignPermissionToUser(int userId, [FromBody] AssingUserPermission dto)
        {
            var (success, errorMessage) = await _rolePermissionService.AssignPermissionsToUserAsync(userId, dto);
            return Json(new
            {
                success,
                message = success
                    ? (dto.IsGranted ? "Permiso concedido al usuario." : "Permiso denegado al usuario.")
                    : errorMessage
            });
        }

        [Authorize(Policy = "usuario.asignar")]
        [HttpPost]
        public async Task<IActionResult> RemoveUserPermission(int userId, int permissionId)
        {
            var (success, errorMessage) = await _rolePermissionService.RemoveUserPermissionAsync(userId, permissionId);
            return Json(new
            {
                success,
                message = success ? "Permiso removido del usuario." : errorMessage
            });
        }

        [Authorize(Policy = "usuario.cambiar_rol")]
        [HttpPost]
        public async Task<IActionResult> UpdateRole(int userId, [FromBody] UpdateUserRoleDto dto)
        {
            var (success, errorMessage) = await _rolePermissionService.UpdateUserRoleAsync(userId, dto.RoleId);
            return Json(new { success, message = success ? "Rol actualizado." : errorMessage });
        }
    }
}