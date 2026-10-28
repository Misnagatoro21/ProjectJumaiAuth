using Microsoft.AspNetCore.Identity.Data;
using ProjectJumaiAuth.DTOS;

namespace ProjectJumaiAuth.Servicces
{
    public interface IAuthServices
    {
        Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterRequestDto dto);
        Task<(bool Success, LoginResponseDto? result, string? ErrorMessage)> LoginAsync(LoginRequestDto dto);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<bool> SendResetPasswordLinkAsync(string email);
        Task<bool> ResetPAsswordAsync(ResetPasswordRequest dto);
        Task<bool> ResendConfirmationEmailAsync(string email);
        Task<bool> CHangePasswordAsync(string userName, ChangePasswordDto dto);
        Task<LoginResponseDto?> RefreshAccessTokenAsync(string refreshToken);
        
    }
}
