namespace API.Controllers.Document;

using API.Models;
using API.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Services;
using API.Contracts.Document;
using API.Mappers;

public class DocumentController : BaseController {
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService) {
        _documentService = documentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequest request) {
        int userId = GetCurrentUserId();

        var dto = await _documentService.CreateDocumentAsync(
            userId,
            request.WorkspaceId,
            request.Title,
            request.Kind,
            request.Visibility,
            request.ColorHex
        );

        var response = DocumentMapper.ToDocumentResponse(dto);

        return Ok(new ApiResponse<DocumentResponse> {
            Success = true,
            Message = "Document created successfully.",
            Data = response
        });
    }

    [HttpGet("{documentId}")]
    public async Task<IActionResult> GetDocument(int documentId) {
        int userId = GetCurrentUserId();

        var dto = await _documentService.GetDocumentAsync(documentId, userId);
        var response = DocumentMapper.ToDocumentResponse(dto);

        return Ok(new ApiResponse<DocumentResponse> {
            Success = true,
            Message = "Document retrieved successfully.",
            Data = response
        });
    }

    [HttpGet("workspace/{workspaceId}")]
    public async Task<IActionResult> GetWorkspaceDocuments(int workspaceId) {
        int userId = GetCurrentUserId();

        var dtos = await _documentService.GetWorkspaceDocumentsAsync(workspaceId, userId);
        var responses = dtos.Select(DocumentMapper.ToDocumentResponse);

        return Ok(new ApiResponse<IEnumerable<DocumentResponse>> {
            Success = true,
            Message = "Workspace documents retrieved successfully.",
            Data = responses
        });
    }

    [HttpPut("{documentId}")]
    public async Task<IActionResult> UpdateDocument(int documentId, [FromBody] UpdateDocumentRequest request) {
        int userId = GetCurrentUserId();

        var dto = await _documentService.UpdateDocumentAsync(documentId, userId, request.Title, request.Content, request.IsArchived, request.ColorHex, request.Visibility);
        var response = DocumentMapper.ToDocumentResponse(dto);

        return Ok(new ApiResponse<DocumentResponse> {
            Success = true,
            Message = "Document updated successfully.",
            Data = response
        });
    }

    [HttpDelete("{documentId}")]
    public async Task<IActionResult> DeleteDocument(int documentId) {
        int userId = GetCurrentUserId();

        await _documentService.DeleteDocumentAsync(documentId, userId);

        return Ok(new ApiResponse<object> {
            Success = true,
            Message = "Document deleted successfully.",
            Data = null
        });
    }
}


