// The key principle: DTOs are the contract between layers, 
// while Domain Models are the internal representation. 
// This separation ensures clean architecture and prevents domain pollution.

namespace API.Controllers.Workspace;

using API.Models;
using API.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Services;
using Core.DTOs;
using API.Contracts.Workspace;
using API.Mappers;

public class WorkspaceController : BaseController {
    private readonly IWorkspaceService _workspaceService;

    public WorkspaceController(IWorkspaceService workspaceService) {
        _workspaceService = workspaceService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceRequest request) {
        int userId = GetCurrentUserId();

        WorkspaceDto workspaceDto = await _workspaceService.CreateWorkspaceAsync(
            userId,
            request.Name,
            request.Visibility,
            request.ColorHex
        );

        WorkspaceResponse response = WorkspaceMapper.ToWorkspaceResponse(workspaceDto);

        return Ok(new ApiResponse<WorkspaceResponse> {
            Success = true,
            Message = "Workspace created successfully.",
            Data = response
        });
    }

    [HttpGet("{workspaceId}")]
    public async Task<IActionResult> GetWorkspace(int workspaceId) {
        int userId = GetCurrentUserId();

        WorkspaceDto? workspaceDto = await _workspaceService.GetWorkspaceAsync(workspaceId, userId);

        WorkspaceResponse response = WorkspaceMapper.ToWorkspaceResponse(workspaceDto!);

        return Ok(new ApiResponse<WorkspaceResponse> {
            Success = true,
            Message = "Workspace retrieved successfully.",
            Data = response
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetUserWorkspaces() {
        int userId = GetCurrentUserId();

        IEnumerable<WorkspaceDto> workspaceDtos = await _workspaceService.GetUserWorkspacesAsync(userId);
        IEnumerable<WorkspaceResponse> responses = workspaceDtos.Select(WorkspaceMapper.ToWorkspaceResponse);

        return Ok(new ApiResponse<IEnumerable<WorkspaceResponse>> {
            Success = true,
            Message = "User workspaces retrieved successfully.",
            Data = responses
        });
    }

    [HttpDelete("{workspaceId}")]
    public async Task<IActionResult> DeleteWorkspace(int workspaceId) {
        int userId = GetCurrentUserId();

        await _workspaceService.DeleteWorkspaceAsync(workspaceId, userId);

        return Ok(new ApiResponse<object> {
            Success = true,
            Message = "Workspace deleted successfully.",
            Data = null
        });
    }

    [HttpPut("{workspaceId}")]
    public async Task<IActionResult> UpdateWorkspace(int workspaceId, [FromBody] UpdateWorkspaceRequest request) {
        int userId = GetCurrentUserId();

        WorkspaceDto workspaceDto = await _workspaceService.UpdateWorkspaceAsync(
            workspaceId,
            userId,
            request.Name,
            request.Description,
            request.Visibility,
            request.ColorHex
        );

        WorkspaceResponse response = WorkspaceMapper.ToWorkspaceResponse(workspaceDto);

        return Ok(new ApiResponse<WorkspaceResponse> {
            Success = true,
            Message = "Workspace updated successfully.",
            Data = response
        });
    }
}