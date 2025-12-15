namespace Core.Services.Document;

using System;
using System.Linq;
using Core.DTOs;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces.repository.Document;
using Core.Interfaces.Services;
using Core.Mappers;
using Core.Models;
using Core.Utils;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Workspace;

public class DocumentService : IDocumentService {
    private readonly IDocumentRepository _documentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserWorkspaceRepository _userWorkspaceRepository;

    public DocumentService(
        IDocumentRepository documentRepository,
        IUserRepository userRepository,
        IUserWorkspaceRepository userWorkspaceRepository) {
        _documentRepository = documentRepository;
        _userRepository = userRepository;
        _userWorkspaceRepository = userWorkspaceRepository;
    }

    public async Task<DocumentDto> CreateDocumentAsync(int creatorId, int workspaceId, string title, DocumentKind kind, WorkspaceVisibility? visibility = null, string? colorHex = null) {
        User creator = await _userRepository.GetByIdAsync(creatorId)
            ?? throw AppException.CreateError("USER_NOT_FOUND");

        bool hasAccess = await _userWorkspaceRepository.UserHasAccessAsync(creatorId, workspaceId);
        if (!hasAccess) {
            throw AppException.CreateError("WORKSPACE_ACCESS_DENIED");
        }

        if (string.IsNullOrWhiteSpace(title)) {
            throw AppException.CreateError("INVALID_DOCUMENT_TITLE");
        }

        string? normalizedColor = ColorUtils.NormalizeColorHex(colorHex);
        string? assignedColor = normalizedColor ?? ColorUtils.GetRandomColorHex();

        string yDocId = Guid.NewGuid().ToString();

        var document = new Document {
            WorkspaceId = workspaceId,
            Title = title.Trim(),
            Kind = kind, 
            YDocId = yDocId,
            Content = null, 
            ColorHex = assignedColor,
            CreatedBy = creatorId,
            IsArchived = false,
            Visibility = visibility ?? WorkspaceVisibility.Public 
        };

        var created = await _documentRepository.CreateAsync(document);

        var withNav = await _documentRepository.GetByIdAsync(created.Id)
            ?? throw AppException.CreateError("DOCUMENT_NOT_FOUND");

        return DocumentMapper.ToDocumentDto(withNav);
    }

    public async Task<DocumentDto> GetDocumentAsync(int documentId, int userId) {
        var doc = await _documentRepository.GetByIdAsync(documentId)
            ?? throw AppException.CreateError("DOCUMENT_NOT_FOUND");

        bool hasAccess = await _userWorkspaceRepository.UserHasAccessAsync(userId, doc.WorkspaceId);
        if (!hasAccess) {
            throw AppException.CreateError("DOCUMENT_ACCESS_DENIED");
        }

        return DocumentMapper.ToDocumentDto(doc);
    }

    public async Task<IEnumerable<DocumentDto>> GetWorkspaceDocumentsAsync(int workspaceId, int userId) {
        bool hasAccess = await _userWorkspaceRepository.UserHasAccessAsync(userId, workspaceId);
        if (!hasAccess) {
            throw AppException.CreateError("WORKSPACE_ACCESS_DENIED");
        }

        var docs = await _documentRepository.GetByWorkspaceIdAsync(workspaceId);
        return docs.Select(DocumentMapper.ToDocumentDto);
    }

    public async Task<bool> DeleteDocumentAsync(int documentId, int userId) {
        var doc = await _documentRepository.GetByIdAsync(documentId)
            ?? throw AppException.CreateError("DOCUMENT_NOT_FOUND");

        bool isOwner = await _userWorkspaceRepository.UserIsOwnerAsync(userId, doc.WorkspaceId);
        if (!isOwner) {
            throw AppException.CreateError("USER_WORKSPACE_INSUFFICIENT_PERMISSIONS");
        }

        return await _documentRepository.DeleteAsync(documentId);
    }

    public async Task<DocumentDto> UpdateDocumentAsync(int documentId, int userId, string? title, string? content, bool? isArchived, string? colorHex, WorkspaceVisibility? visibility = null) {
        var doc = await _documentRepository.GetByIdAsync(documentId)
            ?? throw AppException.CreateError("DOCUMENT_NOT_FOUND");

        bool hasAccess = await _userWorkspaceRepository.UserHasAccessAsync(userId, doc.WorkspaceId);
        if (!hasAccess) {
            throw AppException.CreateError("DOCUMENT_ACCESS_DENIED");
        }

        if (title != null) {
            doc.Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim();
        }

        if (content != null) {
            doc.Content = content;
        }

        if (isArchived.HasValue) {
            doc.IsArchived = isArchived.Value;
        }

        if (colorHex != null) {
            if (string.IsNullOrWhiteSpace(colorHex)) {
                doc.ColorHex = null;
            } else {
                string? normalized = ColorUtils.NormalizeColorHex(colorHex)
                   ?? throw AppException.CreateError("INVALID_COLOR_HEX");
                doc.ColorHex = normalized;
            }
        }

        if (visibility.HasValue) {
            doc.Visibility = visibility.Value;
        }

        await _documentRepository.UpdateAsync(doc);

        var withNav = await _documentRepository.GetByIdAsync(documentId)
            ?? throw AppException.CreateError("DOCUMENT_NOT_FOUND");

        return DocumentMapper.ToDocumentDto(withNav);
    }
}


