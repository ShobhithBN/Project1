using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AIProctoring.Authentication.Models;
using AIProctoring.Authentication.Services;
using AIProctoring.Shared.DTOs;

namespace AIProctoring.Authentication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
            {
                return BadRequest(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid email or password",
                    Errors = new List<string> { "Authentication failed" }
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid email or password",
                    Errors = new List<string> { "Authentication failed" }
                });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Update user's refresh token and last login
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60), // Adjust based on your JWT configuration
                User = new UserProfile
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList(),
                    Institution = user.Institution
                }
            };

            _logger.LogInformation("User {Email} logged in successfully", request.Email);

            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Data = response,
                Message = "Login successful"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return StatusCode(500, new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "An error occurred during login",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserProfile>>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new ApiResponse<UserProfile>
                {
                    Success = false,
                    Message = "User already exists",
                    Errors = new List<string> { "Email is already registered" }
                });
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Institution = request.Institution,
                EmailConfirmed = true // For demo purposes
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<UserProfile>
                {
                    Success = false,
                    Message = "Failed to create user",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                });
            }

            // Add default role
            await _userManager.AddToRoleAsync(user, "Student");

            var userProfile = new UserProfile
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = new List<string> { "Student" },
                Institution = user.Institution
            };

            _logger.LogInformation("User {Email} registered successfully", request.Email);

            return Ok(new ApiResponse<UserProfile>
            {
                Success = true,
                Data = userProfile,
                Message = "Registration successful"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", request.Email);
            return StatusCode(500, new ApiResponse<UserProfile>
            {
                Success = false,
                Message = "An error occurred during registration",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            var email = principal.Identity?.Name;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid token",
                    Errors = new List<string> { "Token validation failed" }
                });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid refresh token",
                    Errors = new List<string> { "Refresh token validation failed" }
                });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenService.GenerateAccessToken(user, roles);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var response = new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserProfile
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList(),
                    Institution = user.Institution
                }
            };

            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Data = response,
                Message = "Token refreshed successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "An error occurred during token refresh",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}