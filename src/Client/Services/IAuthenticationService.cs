using AIProctoring.Shared.DTOs;

namespace AIProctoring.Client.Services;

public interface IAuthenticationService
{
    Task<bool> LoginAsync(string email, string password);
    Task LogoutAsync();
    bool IsAuthenticated { get; }
    UserProfile? CurrentUser { get; }
    string? AccessToken { get; }
    event EventHandler<bool> AuthenticationStateChanged;
}