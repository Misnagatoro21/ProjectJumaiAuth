using ProjectJumaiAuth.Models;

namespace ProjectJumaiAuth.Servicces
{
    public interface ITokenServices
    {
        String GenerateAccessToken(Usuario usuario);
        string GenerateRefreshToken();
        string GeneratePasswordResetToken();

    }
}
