namespace API.Controllers.User;

using System.Collections.Generic;
using System.Linq;
using API.Models;
using Core.DTOs;
using API.Mappers;
using API.Contracts.User;
using Core.Interfaces.Services;
using API.Controllers.Base;
using Microsoft.AspNetCore.Mvc;


public class UserController : BaseController {
    private readonly IUserService _userService;

    public UserController(IUserService userService) {
        _userService = userService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string? query, [FromQuery] int limit = 8) {
        // Allow empty/missing query without triggering [ApiController] model validation.
        IEnumerable<UserDto> users = await _userService.SearchUsers(query ?? string.Empty, limit);

        IEnumerable<UserResponse> response = users.Select(UserMapper.ToGetUserResponse);

        return Ok(new ApiResponse<IEnumerable<UserResponse>> {
            Success = true,
            Message = "Users retrieved successfully.",
            Data = response
        });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile() {
        int userIdResult = GetCurrentUserId();

        UserDto userDto = await _userService.GetByIdUser(userIdResult);

        UserResponse user = UserMapper.ToGetUserResponse(userDto);

        return Ok(new ApiResponse<UserResponse> {
            Success = true,
            Message = "User profile retrieved successfully.",
            Data = user
        });
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] Dictionary<string, object>? updates) {
        if (updates == null || updates.Count == 0) {
            return BadRequest(new ApiResponse<EmptyResponse> {
                Success = false,
                Message = "Invalid request",
                Data = null
            });
        }
        int userId = GetCurrentUserId();

        await _userService.UpdateUser(userId, updates);

        return Ok(new ApiResponse<EmptyResponse> {
            Success = true,
            Message = "User updated successfully.",
            Data = null
        });
    }
}