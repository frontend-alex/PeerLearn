namespace Core.Services.Workspace;

using Core.DTOs;
using Core.Models;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Mappers;
using Core.Exceptions;
using Core.Utils;
using Infrastructure.Repositories;
using Core.Interfaces.repository.workspace;
using Infrastructure.Repositories.Workspace;

public class WorkspaceService : IWorkspaceService {
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUserWorkspaceRepository _userWorkspaceRepository;
    private readonly IUserRepository _userRepository;

    public WorkspaceService(
        IWorkspaceRepository workspaceRepository,
        IUserWorkspaceRepository userWorkspaceRepository,
        IUserRepository userRepository) {
        _workspaceRepository = workspaceRepository;
        _userWorkspaceRepository = userWorkspaceRepository;
        _userRepository = userRepository;
    }

    public async Task<WorkspaceDto> CreateWorkspaceAsync(int creatorId, string name, WorkspaceVisibility visibility, string? colorHex) {
        User creator = await _userRepository.GetByIdAsync(creatorId)
            ?? throw AppException.CreateError("USER_NOT_FOUND");

        string? normalizedColor = ColorUtils.NormalizeColorHex(colorHex);

        Workspace workspace = new Workspace {
            Name = name.Trim(),
            Description = "A collaborative workspace for learning and studying together.",
            Visibility = visibility,
            CreatedBy = creatorId,
            ColorHex = normalizedColor ?? ColorUtils.GetRandomColorHex()
        };

        Workspace createdWorkspace = await _workspaceRepository.CreateAsync(workspace);

        // Add creator as owner
        var userWorkspace = new UserWorkspace {
            UserId = creatorId,
            WorkspaceId = createdWorkspace.Id,
            Role = UserWorkspaceRole.Owner,
            JoinedAt = DateTime.UtcNow
        };

        await _userWorkspaceRepository.CreateAsync(userWorkspace);

        // Update user's onboarding status to true
        creator.Onboarding = true;
        await _userRepository.UpdateAsync(creator);

        // Reload workspace with navigation properties
        Workspace workspaceWithNav = await _workspaceRepository.GetByIdAsync(createdWorkspace.Id)
            ?? throw AppException.CreateError("WORKSPACE_NOT_FOUND");

        return WorkspaceMapper.ToWorkspaceDto(workspaceWithNav, creatorId);
    }

    public async Task<WorkspaceDto?> GetWorkspaceAsync(int workspaceId, int userId) {
        // Check if user has access to workspace
        bool hasAccess = await _userWorkspaceRepository.UserHasAccessAsync(userId, workspaceId);

        if (!hasAccess) {
            throw AppException.CreateError("WORKSPACE_ACCESS_DENIED");
        }

        Workspace workspace = await _workspaceRepository.GetByIdAsync(workspaceId)
            ?? throw AppException.CreateError("WORKSPACE_ACCESS_DENIED");

        return WorkspaceMapper.ToWorkspaceDto(workspace, userId);
    }

    public async Task<IEnumerable<WorkspaceDto>> GetUserWorkspacesAsync(int userId) {
        var workspaces = await _workspaceRepository.GetByUserIdAsync(userId);
        return workspaces.Select(w => WorkspaceMapper.ToWorkspaceDto(w, userId));
    }

    public async Task<bool> DeleteWorkspaceAsync(int workspaceId, int userId) {
        // Check if user is owner
        bool isOwner = await _userWorkspaceRepository.UserIsOwnerAsync(userId, workspaceId);
        if (!isOwner) {
            throw AppException.CreateError("USER_WORKSPACE_INSUFFICIENT_PERMISSIONS");
        }

        return await _workspaceRepository.DeleteAsync(workspaceId);
    }

    public async Task<WorkspaceDto> UpdateWorkspaceAsync(int workspaceId, int userId, string? name, string? description, WorkspaceVisibility? visibility, string? colorHex) {
        // Check if user has access and is owner or cohost
        bool hasAccess = await _userWorkspaceRepository.UserHasAccessAsync(userId, workspaceId);

        if (!hasAccess) {
            throw AppException.CreateError("WORKSPACE_ACCESS_DENIED");
        }

        Workspace workspace = await _workspaceRepository.GetByIdAsync(workspaceId)
            ?? throw AppException.CreateError("WORKSPACE_NOT_FOUND");

        if (!string.IsNullOrWhiteSpace(name)) {
            workspace.Name = name.Trim();
        }

        if (description != null) {
            workspace.Description = description.Trim();
        }

        if (visibility.HasValue) {
            workspace.Visibility = visibility.Value;
        }

        if (colorHex != null) {
            if (string.IsNullOrWhiteSpace(colorHex)) {
                workspace.ColorHex = null;
            } else {
                string? normalized = ColorUtils.NormalizeColorHex(colorHex);
                if (normalized is null) {
                    throw AppException.CreateError("INVALID_COLOR_HEX");
                }
                workspace.ColorHex = normalized;
            }
        }

        await _workspaceRepository.UpdateAsync(workspace);

        // Reload workspace with navigation properties
        Workspace updatedWorkspaceWithNav = await _workspaceRepository.GetByIdAsync(workspaceId)
            ?? throw AppException.CreateError("WORKSPACE_NOT_FOUND");

        return WorkspaceMapper.ToWorkspaceDto(updatedWorkspaceWithNav, userId);
    }
}
