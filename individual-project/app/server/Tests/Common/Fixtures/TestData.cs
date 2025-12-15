using API.Models.Auth;
using API.Contracts.Document;
using API.Contracts.User;
using Core.Enums;

namespace PeerLearn.Tests.Common.Fixtures;

/// <summary>
/// Provides test data factories for common request/response objects
/// </summary>
public static class TestData
{
    public static class Auth
    {
        public static RegisterRequest CreateRegisterRequest(string? email = null, string? username = null)
        {
            return new RegisterRequest
            {
                Username = username ?? $"testuser_{Guid.NewGuid():N}",
                FirstName = "Test",
                LastName = "User",
                Email = email ?? $"test_{Guid.NewGuid():N}@example.com",
                Password = "Password123!"
            };
        }

        public static LoginRequest CreateLoginRequest(string email, string password = "Password123!")
        {
            return new LoginRequest
            {
                Email = email,
                Password = password
            };
        }

        public static SendOtpRequest CreateSendOtpRequest(string email)
        {
            return new SendOtpRequest
            {
                Email = email
            };
        }

        public static OtpVerifyRequest CreateOtpVerifyRequest(string email, string code = "123456")
        {
            return new OtpVerifyRequest
            {
                Email = email,
                Code = code
            };
        }
    }

    public static class Document
    {
        public static CreateDocumentRequest CreateDocumentRequest(
            int workspaceId,
            string? title = null,
            DocumentKind kind = DocumentKind.Document)
        {
            return new CreateDocumentRequest
            {
                WorkspaceId = workspaceId,
                Title = title ?? "Test Document",
                Kind = kind,
                Visibility = WorkspaceVisibility.Public,
                ColorHex = "#FFFFFF"
            };
        }

        public static UpdateDocumentRequest CreateUpdateDocumentRequest(
            string? title = null,
            string? content = null,
            bool? isArchived = null)
        {
            return new UpdateDocumentRequest
            {
                Title = title,
                Content = content,
                IsArchived = isArchived,
                Visibility = WorkspaceVisibility.Public,
                ColorHex = "#FFFFFF"
            };
        }
    }

    public static class User
    {
        public static Dictionary<string, object> CreateUpdateRequest(string? firstName = null, string? lastName = null)
        {
            var updates = new Dictionary<string, object>();
            if (firstName != null) updates["FirstName"] = firstName;
            if (lastName != null) updates["LastName"] = lastName;
            return updates;
        }
    }
}

