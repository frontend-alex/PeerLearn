using Core.Models;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.SQL;
using Core.Interfaces.repository.workspace;
using WorkspaceModel = Core.Models.Workspace;

namespace Infrastructure.Repositories.Workspace;

public class WorkspaceRepository : IWorkspaceRepository{
    private readonly ApplicationDbContext _context;

    public WorkspaceRepository(ApplicationDbContext context) {
        _context = context;
    }

    public async Task<WorkspaceModel?> GetByIdAsync(int id) {
        return await _context.Workspaces
            .Include(w => w.Creator)
            .Include(w => w.UserWorkspaces)
            .Include(w => w.Documents)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<WorkspaceModel> CreateAsync(WorkspaceModel workspace) {
        _context.Workspaces.Add(workspace);
        await _context.SaveChangesAsync();
        return workspace;
    }

    public async Task<WorkspaceModel> UpdateAsync(WorkspaceModel workspace) {
        _context.Workspaces.Update(workspace);
        await _context.SaveChangesAsync();
        return workspace;
    }

    public async Task<bool> DeleteAsync(int id) {
        WorkspaceModel? workspace = await _context.Workspaces.FindAsync(id);
        if (workspace == null) return false;

        _context.Workspaces.Remove(workspace);

        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<IEnumerable<WorkspaceModel>> GetByUserIdAsync(int userId) {
        return await _context.Workspaces
            .Include(w => w.Creator)
            .Include(w => w.UserWorkspaces)
            .Include(w => w.Documents)
            .Where(w => w.UserWorkspaces.Any(uw => uw.UserId == userId))
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id) {
        return await _context.Workspaces
            .AnyAsync(w => w.Id == id);
    }
}


