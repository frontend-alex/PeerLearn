namespace Core.Interfaces.Services;

using Core.DTOs;
using Core.Enums;

public interface IWorkspaceService {
    Task<WorkspaceDto> CreateWorkspaceAsync(int creatorId, string name, WorkspaceVisibility visibility, string? colorHex);
    Task<WorkspaceDto> GetWorkspaceAsync(int workspaceId, int userId);
    Task<IEnumerable<WorkspaceDto>> GetUserWorkspacesAsync(int userId);
    Task<WorkspaceDto> UpdateWorkspaceAsync(int workspaceId, int userId, string? name, string? description, WorkspaceVisibility? visibility, string? colorHex);
    Task<bool> DeleteWorkspaceAsync(int workspaceId, int userId);
}

