using Core.Models;
using Core.Enums;

namespace PeerLearn.Tests.Common.Builders;

/// <summary>
/// Builder pattern for creating test Document entities
/// </summary>
public class DocumentBuilder
{
    private Document _document;

    public DocumentBuilder()
    {
        _document = new Document
        {
            Title = "Test Document",
            WorkspaceId = 1,
            Kind = DocumentKind.Document,
            YDocId = Guid.NewGuid().ToString(),
            Content = null,
            ColorHex = "#FFFFFF",
            CreatedBy = 1,
            IsArchived = false,
            Visibility = WorkspaceVisibility.Public
        };
    }

    public DocumentBuilder WithId(int id)
    {
        _document.Id = id;
        return this;
    }

    public DocumentBuilder WithTitle(string title)
    {
        _document.Title = title;
        return this;
    }

    public DocumentBuilder WithWorkspaceId(int workspaceId)
    {
        _document.WorkspaceId = workspaceId;
        return this;
    }

    public DocumentBuilder WithKind(DocumentKind kind)
    {
        _document.Kind = kind;
        return this;
    }

    public DocumentBuilder WithContent(string? content)
    {
        _document.Content = content;
        return this;
    }

    public DocumentBuilder WithColorHex(string? colorHex)
    {
        _document.ColorHex = colorHex;
        return this;
    }

    public DocumentBuilder WithCreatedBy(int userId)
    {
        _document.CreatedBy = userId;
        return this;
    }

    public DocumentBuilder WithIsArchived(bool archived)
    {
        _document.IsArchived = archived;
        return this;
    }

    public DocumentBuilder WithVisibility(WorkspaceVisibility visibility)
    {
        _document.Visibility = visibility;
        return this;
    }

    public Document Build() => _document;
}

