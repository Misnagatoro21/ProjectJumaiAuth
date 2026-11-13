using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using ProjectJumaiAuth.DTOS;
using ProjectJumaiAuth.Models;
using ProjectJumaiAuth.Repositories;
using System.Security.Cryptography;
using ProjectJumaiAuth.Services;
using ProjectJumaiAuth.Servicces;
namespace ProjectJumaiAuth.Services
{
    public class AuthServices(IUsuarioRepository userRepo, IRoleRepository roleRepo, ITokenServices tokenService, IEmailService emailService, IMapper mapper) : IAuthServices
    {
        private readonly IUsuarioRepository _userRepo = userRepo;
        private readonly IRoleRepository _roleRepo = roleRepo;
        private readonly IEmailService _emailService = emailService;
        private readonly ITokenServices _tokenService = tokenService;
        private readonly IMapper _mapper = mapper;

        public async Task<bool> ChangePasswordAsync(string userName, ChangePasswordDto dto)
        {
            var user = await _userRepo.GetUsuarioByUserName(userName);
            if (user == null || !_userRepo.ValidatePassword(user, dto.CurrentPassword))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.NewPassword);
            await _userRepo.SaveAsync();
            return true;
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userRepo.GetUsuarioByEmail(email);
            if (user == null || string.IsNullOrWhiteSpace(user.EmailConfirmToken))
                return false;

            var providedToken = Uri.UnescapeDataString(token.Trim());
            if (!string.Equals(user.EmailConfirmToken, providedToken, StringComparison.Ordinal))
                return false;

            user.EmailConfirmed = true;
            user.EmailConfirmToken = null;
            await _userRepo.SaveAsync();
            return true;
        }

        public async Task<(bool Success, LoginResponseDto? result, string? ErrorMessage)> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepo.GetUsuarioByEmail(dto.Email);
            if (user == null || !_userRepo.ValidatePassword(user, dto.Password))
                return (false, null, "Credenciales invalidas");
            if (!user.EmailConfirmed)
                return (false, null, "Debe confirmar su correo electronico");

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshExpires = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshExpires;
            await _userRepo.SaveAsync();

            var result = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = refreshExpires
            };
            return (true, result, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterRequestDto dto)
        {
            var exist = await _userRepo.GetUsuarioByEmail(dto.Email);
            if (exist != null)
                return (false, "Correo ya en uso, Intente con otro correo");

            var usuarioRole = await _roleRepo.GetByNameAsync("Usuario");
            if (usuarioRole == null)
                return (false, "El rol Usuario no esta configurado.");

            var user = _mapper.Map<Usuario>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password);
            user.RolId = usuarioRole.Id;

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var rawToken = Convert.ToBase64String(tokenBytes);
            var encodedToken = Uri.EscapeDataString(rawToken);

            user.EmailConfirmToken = rawToken;
            user.EmailConfirmed = false;
            await _userRepo.CreateAsync(user);
            await _userRepo.SaveAsync();

            var encodedEmail = Uri.EscapeDataString(user.Email);
            string confirmationLink = $"https://localhost:7159/Auth/ConfirmEmail?email={encodedEmail}&token={encodedToken}";
            string htmlBody = $"""
                <!DOCTYPE html>
                <html><body>
                <p>Hola,</p>
                <p>Haz clic en el siguiente botón para confirmar tu cuenta:</p>
                <p><a href="{confirmationLink}" style='background:#007BFF;color:white;padding:10px 20px;border-radius:5px;text-decoration:none;'>Confirmar Cuenta</a></p>
                </body></html>
                """;
            _emailService.SendPasswordResetEmail(user.Email, htmlBody);
            return (true, null);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto dto)
            {
            var user = await _userRepo.GetUsuarioByEmail(dto.Email);
            if (user == null)
                return false;

            var providedToken = Uri.UnescapeDataString(dto.Token ?? string.Empty);
            var storedToken = user.PasswordResetToken ?? string.Empty;

            // Compara el token guardado en la BD con el que envía la app
            if (!storedToken.Equals(providedToken, StringComparison.Ordinal))
                return false;

            if (!user.PasswordResetTokenExpiryTime.HasValue || user.PasswordResetTokenExpiryTime.Value < DateTime.UtcNow)
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiryTime = null;
            await _userRepo.SaveAsync();
            return true;
        }

        public async Task<bool> SendResetPasswordLinkAsync(string email)
        {
            var user = await _userRepo.GetUsuarioByEmail(email);
            if (user == null) return false;

            var token = _tokenService.GeneratePasswordResetToken();
            var encodedToken = Uri.EscapeDataString(token);
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiryTime = DateTime.UtcNow.AddHours(1);
            await _userRepo.SaveAsync();

            var encodedEmail = Uri.EscapeDataString(user.Email);
            string resetLink = $"https://localhost:7159/Auth/ResetPassword?email={encodedEmail}&token={encodedToken}";
            string htmlBody = $"""
                <!DOCTYPE html>
                <html><body>
                <p>Hola,</p>
                <p>Haz clic en el siguiente botón para restablecer tu contraseña:</p>
                <p><a href="{resetLink}" style='background:#007BFF;color:white;padding:10px 20px;border-radius:5px;text-decoration:none;'>Restablecer contraseña</a></p>
                </body></html>
                """;
            _emailService.SendPasswordResetEmail(user.Email, htmlBody);
            return true;
        }

        public async Task<LoginResponseDto?> RefreshAccessTokenAsync(string refreshToken)
        {
            var user = await _userRepo.GetUsuarioByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return null;

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var refreshExpires = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = refreshExpires;
            await _userRepo.SaveAsync();

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = refreshExpires
            };
        }
    }
}