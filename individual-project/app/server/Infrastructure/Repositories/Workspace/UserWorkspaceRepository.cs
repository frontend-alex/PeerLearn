using Core.Models;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.SQL;

namespace Infrastructure.Repositories.Workspace;


public class UserWorkspaceRepository : IUserWorkspaceRepository {
    private readonly ApplicationDbContext _context;

    public UserWorkspaceRepository(ApplicationDbContext context) {
        _context = context;
    }

    public async Task<UserWorkspace> CreateAsync(UserWorkspace userWorkspace) {
        _context.UserWorkspaces.Add(userWorkspace);
        await _context.SaveChangesAsync();
        return userWorkspace;
    }

    public async Task<bool> DeleteAsync(int userId, int workspaceId) {
        var userWorkspace = await _context.UserWorkspaces
            .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkspaceId == workspaceId);
        
        if (userWorkspace == null) return false;

        _context.UserWorkspaces.Remove(userWorkspace);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserWorkspace?> GetAsync(int userId, int workspaceId) {
        return await _context.UserWorkspaces
            .Include(uw => uw.User)
            .Include(uw => uw.Workspace)
            .FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WorkspaceId == workspaceId);
    }

    public async Task<IEnumerable<UserWorkspace>> GetByWorkspaceIdAsync(int workspaceId) {
        return await _context.UserWorkspaces
            .Include(uw => uw.User)
            .Include(uw => uw.Workspace)
            .Where(uw => uw.WorkspaceId == workspaceId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserWorkspace>> GetByUserIdAsync(int userId) {
        return await _context.UserWorkspaces
            .Include(uw => uw.User)
            .Include(uw => uw.Workspace)
            .Where(uw => uw.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> UserHasAccessAsync(int userId, int workspaceId) {
        return await _context.UserWorkspaces
            .AnyAsync(uw => uw.UserId == userId && uw.WorkspaceId == workspaceId);
    }

    public async Task<bool> UserIsOwnerAsync(int userId, int workspaceId) {
        return await _context.UserWorkspaces
            .AnyAsync(uw => uw.UserId == userId && 
                           uw.WorkspaceId == workspaceId && 
                           uw.Role == Core.Enums.UserWorkspaceRole.Owner);
    }
}


