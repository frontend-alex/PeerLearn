namespace Core.Interfaces.Services;

using Core.DTOs;
using Core.Enums;

public interface IDocumentService {
    Task<DocumentDto> CreateDocumentAsync(int creatorId, int workspaceId, string title, DocumentKind kind, WorkspaceVisibility? visibility = null, string? colorHex = null);
    Task<DocumentDto> GetDocumentAsync(int documentId, int userId);
    Task<IEnumerable<DocumentDto>> GetWorkspaceDocumentsAsync(int workspaceId, int userId);
    Task<bool> DeleteDocumentAsync(int documentId, int userId);
    Task<DocumentDto> UpdateDocumentAsync(int documentId, int userId, string? title, string? content, bool? isArchived, string? colorHex, WorkspaceVisibility? visibility = null);
}

