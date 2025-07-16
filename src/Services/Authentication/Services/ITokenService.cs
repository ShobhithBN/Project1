using AIProctoring.Authentication.Models;
using System.Security.Claims;

namespace AIProctoring.Authentication.Services;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    bool ValidateRefreshToken(string refreshToken);
}