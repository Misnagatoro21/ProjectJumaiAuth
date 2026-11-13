using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectJumaiAuth.Models.Settings;
using ProjectJumaiAuth.Servicces;
using ProjectJumaiAuth.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ProjectJumaiAuth.Middleware
{
    public class JwtRefreshMiddleware(
        RequestDelegate next,
        IOptions<JwtSettings> jwtSettings)
    {
        private readonly RequestDelegate _next = next;
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public async Task InvokeAsync(
            HttpContext context,
            IAuthServices authService)
        {
            // Evitar validaciones innecesarias en rutas públicas
            if (context.Request.Path.StartsWithSegments("/Auth"))
            {
                await _next(context);
                return;
            }

            var accessToken = context.Request.Cookies["access_token"];
            var refreshToken = context.Request.Cookies["refresh_token"];

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var handler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_jwtSettings.Key)
                    ),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                try
                {
                    // Validar token
                    var principal = handler.ValidateToken(
                        accessToken,
                        validationParameters,
                        out SecurityToken validatedToken);

                    // Validar algoritmo explícitamente
                    if (validatedToken is JwtSecurityToken jwtToken &&
                        !jwtToken.Header.Alg.Equals(
                            SecurityAlgorithms.HmacSha256,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        throw new SecurityTokenException("Algoritmo inválido.");
                    }

                    // Usuario autenticado para esta petición
                    context.User = principal;
                }
                catch (SecurityTokenExpiredException)
                {
                    if (string.IsNullOrWhiteSpace(refreshToken))
                    {
                        await HandleInvalidToken(context);
                        return;
                    }

                    var newTokens =
                        await authService.RefreshAccessTokenAsync(refreshToken);

                    if (newTokens is null)
                    {
                        await HandleInvalidToken(context);
                        return;
                    }

                    // Actualizar Access Token
                    context.Response.Cookies.Append(
                        "access_token",
                        newTokens.AccessToken,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = newTokens.ExpiresAt
                        });

                    // Actualizar Refresh Token
                    context.Response.Cookies.Append(
                        "refresh_token",
                        newTokens.RefreshToken,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = newTokens.RefreshTokenExpiresAt
                        });

                    try
                    {
                        // Reconstruir el usuario para esta misma petición
                        var principal = handler.ValidateToken(
                            newTokens.AccessToken,
                            validationParameters,
                            out SecurityToken validatedToken);

                        if (validatedToken is JwtSecurityToken jwtToken &&
                            !jwtToken.Header.Alg.Equals(
                                SecurityAlgorithms.HmacSha256,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            throw new SecurityTokenException("Algoritmo inválido.");
                        }

                        context.User = principal;
                    }
                    catch
                    {
                        await HandleInvalidToken(context);
                        return;
                    }
                }
                catch (SecurityTokenException)
                {
                    await HandleInvalidToken(context);
                    return;
                }
                catch
                {
                    await HandleInvalidToken(context);
                    return;
                }
            }

            await _next(context);
        }

        private static Task HandleInvalidToken(HttpContext context)
        {
            var options = new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            context.Response.Cookies.Delete("access_token", options);
            context.Response.Cookies.Delete("refresh_token", options);
            context.Response.Redirect("/Auth/Login");

            return Task.CompletedTask;
        }
    }
}