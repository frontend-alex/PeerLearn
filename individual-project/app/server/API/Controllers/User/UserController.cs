namespace API.Controllers.User;

using System.Collections.Generic;
using System.Linq;
using API.Models;
using Core.DTOs;
using API.Mappers;
using API.Contracts.User;
using Core.Services.User;
using API.Controllers.Base;
using Microsoft.AspNetCore.Mvc;


public class UserController : BaseController {
    private readonly UserService _userService;

    public UserController(UserService userService) {
        _userService = userService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string query, [FromQuery] int limit = 8) {
        IEnumerable<UserDto> users = await _userService.SearchUsers(query, limit);

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
    public async Task<IActionResult> UpdateUser([FromBody] Dictionary<string, object> updates) {
        int userId = GetCurrentUserId();

        await _userService.UpdateUser(userId, updates);

        return Ok(new ApiResponse<EmptyResponse> {
            Success = true,
            Message = "User updated successfully.",
            Data = null
        });
    }
}