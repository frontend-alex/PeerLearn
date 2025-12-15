using API.Models;
using API.Models.Auth;
using Core.Services.Auth;
using Core.Interfaces.Services;
using API.Contracts.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest? request) {
        if (request == null) {
            return BadRequest(new ApiResponse<EmptyResponse> {
                Success = false,
                Message = "Invalid request",
                Data = null
            });
        }
        string email = await _authService.RegisterAsync(request.Username, request.FirstName, request.LastName, request.Email, request.Password);

        return Ok(new ApiResponse<RegisterResponse> {
            Success = true,
            Message = "User has successfully registered.",
            Data =  new RegisterResponse { Email = email } 
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest? request) {
        if (request == null) {
            return BadRequest(new ApiResponse<EmptyResponse> {
                Success = false,
                Message = "Invalid request",
                Data = null
            });
        }
        string token = await _authService.LoginAsync(request.Email, request.Password);

        Response.Cookies.Append("access_token", token, new CookieOptions {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        return Ok(new ApiResponse<EmptyResponse> {
            Success = true,
            Message = "User has successfully logged in.",
            Data = null
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout() {
        Response.Cookies.Delete("access_token");

        return Ok(new ApiResponse<EmptyResponse> {
            Success = true,
            Message = "Logged out successfully.",
            Data = null
        });
    }
}
