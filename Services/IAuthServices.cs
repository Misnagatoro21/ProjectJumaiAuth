using ProjectJumaiAuth.DTOS;

namespace ProjectJumaiAuth.Services
{
    public interface IAuthServices
    {
        Task<bool> ChangePasswordAsync(string userName, ChangePasswordDto dto);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<(bool Success, LoginResponseDto? result, string? ErrorMessage)> LoginAsync(LoginRequestDto dto);
        Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterRequestDto dto);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto dto);
        Task<bool> SendResetPasswordLinkAsync(string email);
        Task<LoginResponseDto?> RefreshAccessTokenAsync(string refreshToken);
    }
}