using API.Contracts.Auth;
using API.Models;
using API.Models.Auth;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class OtpController : ControllerBase {
    private readonly IOtpService _otpService;

    public OtpController(IOtpService otpService) {
        _otpService = otpService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest? request) {
        if (request == null || string.IsNullOrWhiteSpace(request.Email)) {
            return BadRequest(new ApiResponse<EmptyResponse> {
                Success = false,
                Message = "Invalid request",
                Data = null
            });
        }
        DateTime expirationTime = await _otpService.SendOtpAsync(request.Email);
        return Ok(new ApiResponse<OtpResponse> {
            Success = true,
            Message = "OTP sent successfully",
            Data = new OtpResponse { ExpiresAt = expirationTime }
        });
    }

    [HttpPut("verify")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest? request) {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Code)) {
            return BadRequest(new ApiResponse<EmptyResponse> {
                Success = false,
                Message = "Invalid request",
                Data = null
            });
        }
        await _otpService.VerifyOtpAsync(request.Email, request.Code);
        return Ok(new ApiResponse<EmptyResponse> {
            Success = true,
            Message = "OTP verified successfully",
            Data = null
        });
    }
}
