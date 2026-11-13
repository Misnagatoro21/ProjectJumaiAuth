using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Services;

namespace ProjectJumaiAuth.Controllers
{
    public class AuthController(IAuthServices authService) : Controller
    {
        private readonly IAuthServices _authService = authService;

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Hubo un error al registrar el usuario");

            var (success, error) = await _authService.RegisterAsync(dto);
            if (!success)
            {
                return Conflict(new
                {
                    message = error
                });
            }
            return Ok(new { success = true });
        }

        [HttpGet]
        public IActionResult Login()
        {
            var token = Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errorMessage = "Datos Invalidos" });

            var (success, result, error) = await _authService.LoginAsync(dto);
            if (!success || result is null)
                return Unauthorized(new { success = false, errorMessage = error ?? "Credenciales invalidas" });

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.ExpiresAt
            };
            Response.Cookies.Append("access_token", result.AccessToken, cookieOptions);
            Response.Cookies.Append("refresh_token", result.RefreshToken, cookieOptions);

            return Ok(new
            {
                success = true,
                message = "Login Exitoso!"
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Json(new { success = true, message = "Sesion cerrada exitosamente" });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Parametros invalidos");

            var confirmado = await _authService.ConfirmEmailAsync(email, token);
            if (confirmado)
            {
                TempData["success"] = "Correo confirmado ya puedes iniciar session";
                return RedirectToAction("Login");
            }
            TempData["error"] = "El token es invalido o ya expiro";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest(new { success = false, errorMessage = "El correo es requerido" });
            }

            var sent = await _authService.SendResetPasswordLinkAsync(model.Email);
            if (sent)
            {
                return Ok(new { success = true });
            }
            else
            {
                return NotFound(new { success = false, errorMessage = " correo no encontrado." });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordRequestDto { Email = email, Token = token };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos inválidos" });

            var success = await _authService.ResetPasswordAsync(dto);
            if (success)
                return Ok(new { success = true });

            return BadRequest(new { success = false, message = "Token inválido o expirado" });
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword() => View();

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            var userName = User.Identity?.Name!;
            var success = await _authService.ChangePasswordAsync(userName, dto);
            if (success)
            {
                TempData["Success"] = "Contraseña actualizada";
                return RedirectToAction("Logout");
            }
            ModelState.AddModelError("", "Contraseña actual incorrecta");
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();
            var newToken = await _authService.RefreshAccessTokenAsync(refreshToken);
            if (newToken == null)
                return Unauthorized();

            var accessOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newToken.ExpiresAt
            
        };
            var refreshOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("access_token", newToken.AccessToken, accessOptions);
            Response.Cookies.Append("refresh_token", newToken.RefreshToken, refreshOptions);
            return Ok();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}