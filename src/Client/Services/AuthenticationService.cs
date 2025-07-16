using AIProctoring.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace AIProctoring.Client.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IApiService _apiService;
    private readonly ILogger<AuthenticationService> _logger;
    private UserProfile? _currentUser;
    private string? _accessToken;

    public AuthenticationService(IApiService apiService, ILogger<AuthenticationService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken) && _currentUser != null;

    public UserProfile? CurrentUser => _currentUser;

    public string? AccessToken => _accessToken;

    public event EventHandler<bool>? AuthenticationStateChanged;

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await _apiService.LoginAsync(loginRequest);

            if (response.Success && response.Data != null)
            {
                _accessToken = response.Data.AccessToken;
                _currentUser = response.Data.User;
                
                // Set the auth token in the API service
                _apiService.SetAuthToken(_accessToken);

                // Store tokens securely (you might want to use secure storage)
                await SecureStorage.SetAsync("access_token", _accessToken);
                await SecureStorage.SetAsync("refresh_token", response.Data.RefreshToken);

                _logger.LogInformation("User {Email} logged in successfully", email);
                AuthenticationStateChanged?.Invoke(this, true);
                return true;
            }
            else
            {
                _logger.LogWarning("Login failed for {Email}: {Message}", email, response.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", email);
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            // Clear stored tokens
            SecureStorage.RemoveAll();
            
            _accessToken = null;
            _currentUser = null;
            
            _logger.LogInformation("User logged out");
            AuthenticationStateChanged?.Invoke(this, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
        }
    }

    public async Task<bool> TryRestoreSessionAsync()
    {
        try
        {
            var accessToken = await SecureStorage.GetAsync("access_token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                _accessToken = accessToken;
                _apiService.SetAuthToken(_accessToken);
                
                // You might want to validate the token with the server here
                // For now, we'll assume it's valid if it exists
                
                AuthenticationStateChanged?.Invoke(this, true);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring session");
        }
        
        return false;
    }
}