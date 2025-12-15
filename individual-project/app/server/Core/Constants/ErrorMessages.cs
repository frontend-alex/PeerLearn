namespace Core.Constants;

public static class ErrorMessages
{
    public static readonly Dictionary<string, ErrorInfo> Messages = new()
    {
        // Authentication errors
        ["UNAUTHORIZED_ACCESS"] = new()
        {
            Message = "User authentication required.",
            StatusCode = 401,
            ErrorCode = "AUTH_001",
            UserFriendlyMessage = "You must be logged in to access this resource."
        },
        ["FORBIDDEN_ACCESS"] = new()
        {
            Message = "You do not have permission to access this resource.",
            StatusCode = 403,
            ErrorCode = "AUTH_002",
            UserFriendlyMessage = "Access denied. You do not have admin privileges."
        },
        ["INVALID_CREDENTIALS"] = new()
        {
            Message = "Invalid email or password.",
            StatusCode = 401,
            ErrorCode = "AUTH_003",
            UserFriendlyMessage = "Your email or password is incorrect. Try again or reset your password."
        },
        ["EMAIL_NOT_VERIFIED"] = new()
        {
            Message = "Email is not verified.",
            StatusCode = 400,
            ErrorCode = "AUTH_004",
            UserFriendlyMessage = "Please verify your email before logging in."
        },

        // User errors
        ["EMAIL_EXISTS"] = new()
        {
            Message = "This email is already registered.",
            StatusCode = 409,
            ErrorCode = "USER_001",
            UserFriendlyMessage = "An account with this email already exists. Please log in or use a different email."
        },
        ["EMAIL_EXISTS_NOT_VERIFIED"] = new()
        {
            Message = "This email is already registered but not verified.",
            StatusCode = 400,
            ErrorCode = "USER_001A",
            UserFriendlyMessage = "An account with this email already exists but is not verified. Please verify your email to continue."
        },
        ["USER_NOT_FOUND"] = new()
        {
            Message = "User account not found.",
            StatusCode = 404,
            ErrorCode = "USER_002",
            UserFriendlyMessage = "We couldn't find your account. Please check your email or sign up."
        },
        ["USERNAME_EXISTS"] = new()
        {
            Message = "This username is already taken.",
            StatusCode = 409,
            ErrorCode = "USER_003",
            UserFriendlyMessage = "That username is already taken. Please choose a different one."
        },
        ["USER_ALREADY_VERIFIED"] = new()
        {
            Message = "User email is already verified.",
            StatusCode = 409,
            ErrorCode = "USER_004",
            UserFriendlyMessage = "Your email has already been verified. You can log in directly."
        },
        ["USER_LOCKED"] = new()
        {
            Message = "User account is locked due to multiple failed login attempts.",
            StatusCode = 423, // Locked
            ErrorCode = "USER_005",
            UserFriendlyMessage = "Your account is temporarily locked. Please reset your password or try later."
        },
        ["PASSWORD_WEAK"] = new()
        {
            Message = "Password does not meet security requirements.",
            StatusCode = 400,
            ErrorCode = "USER_006",
            UserFriendlyMessage = "Your password is too weak. Please choose a stronger password with letters, numbers, and symbols."
        },
        ["PASSWORD_MISMATCH"] = new()
        {
            Message = "Passwords do not match.",
            StatusCode = 400,
            ErrorCode = "USER_007",
            UserFriendlyMessage = "Your password and confirmation password must match."
        },
        ["USER_DELETE_FAILED"] = new()
        {
            Message = "Failed to delete user account.",
            StatusCode = 500,
            ErrorCode = "USER_008",
            UserFriendlyMessage = "We couldn't delete your account. Please try again or contact support."
        },

        // Workspace errors
        ["WORKSPACE_NOT_FOUND"] = new()
        {
            Message = "Workspace not found.",
            StatusCode = 404,
            ErrorCode = "WORKSPACE_001",
            UserFriendlyMessage = "The workspace you're looking for doesn't exist or has been deleted."
        },
        ["WORKSPACE_ACCESS_DENIED"] = new()
        {
            Message = "You do not have access to this workspace.",
            StatusCode = 403,
            ErrorCode = "WORKSPACE_002",
            UserFriendlyMessage = "You don't have permission to access this workspace. Ask the owner to invite you."
        },
        ["WORKSPACE_NAME_REQUIRED"] = new()
        {
            Message = "Workspace name is required.",
            StatusCode = 400,
            ErrorCode = "WORKSPACE_003",
            UserFriendlyMessage = "Please provide a name for your workspace."
        },
        ["WORKSPACE_NAME_TOO_LONG"] = new()
        {
            Message = "Workspace name is too long.",
            StatusCode = 400,
            ErrorCode = "WORKSPACE_004",
            UserFriendlyMessage = "Workspace name must be 128 characters or less."
        },
        ["WORKSPACE_DESCRIPTION_TOO_LONG"] = new()
        {
            Message = "Workspace description is too long.",
            StatusCode = 400,
            ErrorCode = "WORKSPACE_005",
            UserFriendlyMessage = "Workspace description must be 1000 characters or less."
        },
        ["WORKSPACE_OWNER_REQUIRED"] = new()
        {
            Message = "Workspace must have an owner.",
            StatusCode = 400,
            ErrorCode = "WORKSPACE_006",
            UserFriendlyMessage = "Every workspace must have an owner."
        },
        ["WORKSPACE_DELETE_FAILED"] = new()
        {
            Message = "Failed to delete workspace.",
            StatusCode = 500,
            ErrorCode = "WORKSPACE_007",
            UserFriendlyMessage = "We couldn't delete the workspace. Please try again or contact support."
        },
        ["WORKSPACE_UPDATE_FAILED"] = new()
        {
            Message = "Failed to update workspace.",
            StatusCode = 500,
            ErrorCode = "WORKSPACE_008",
            UserFriendlyMessage = "We couldn't update the workspace. Please try again or contact support."
        },

        // UserWorkspace errors
        ["USER_WORKSPACE_NOT_FOUND"] = new()
        {
            Message = "User workspace relationship not found.",
            StatusCode = 404,
            ErrorCode = "USER_WORKSPACE_001",
            UserFriendlyMessage = "You are not a member of this workspace."
        },
        ["USER_WORKSPACE_ALREADY_EXISTS"] = new()
        {
            Message = "User is already a member of this workspace.",
            StatusCode = 409,
            ErrorCode = "USER_WORKSPACE_002",
            UserFriendlyMessage = "You are already a member of this workspace."
        },
        ["USER_WORKSPACE_INSUFFICIENT_PERMISSIONS"] = new()
        {
            Message = "Insufficient permissions for this workspace action.",
            StatusCode = 403,
            ErrorCode = "USER_WORKSPACE_003",
            UserFriendlyMessage = "You don't have the required permissions to perform this action."
        },
        ["USER_WORKSPACE_OWNER_CANNOT_LEAVE"] = new()
        {
            Message = "Workspace owner cannot leave the workspace.",
            StatusCode = 400,
            ErrorCode = "USER_WORKSPACE_004",
            UserFriendlyMessage = "As the workspace owner, you cannot leave. Transfer ownership first or delete the workspace."
        },
        ["USER_WORKSPACE_JOIN_FAILED"] = new()
        {
            Message = "Failed to join workspace.",
            StatusCode = 500,
            ErrorCode = "USER_WORKSPACE_005",
            UserFriendlyMessage = "We couldn't add you to the workspace. Please try again or contact support."
        },
        ["USER_WORKSPACE_LEAVE_FAILED"] = new()
        {
            Message = "Failed to leave workspace.",
            StatusCode = 500,
            ErrorCode = "USER_WORKSPACE_006",
            UserFriendlyMessage = "We couldn't remove you from the workspace. Please try again or contact support."
        },

        // Workspace Invitation errors
        ["WORKSPACE_INVITATION_NOT_FOUND"] = new()
        {
            Message = "Workspace invitation not found.",
            StatusCode = 404,
            ErrorCode = "INVITATION_001",
            UserFriendlyMessage = "This invitation doesn't exist or has expired."
        },
        ["WORKSPACE_INVITATION_EXPIRED"] = new()
        {
            Message = "Workspace invitation has expired.",
            StatusCode = 400,
            ErrorCode = "INVITATION_002",
            UserFriendlyMessage = "This invitation has expired. Please request a new one."
        },
        ["WORKSPACE_INVITATION_ALREADY_ACCEPTED"] = new()
        {
            Message = "Workspace invitation has already been accepted.",
            StatusCode = 409,
            ErrorCode = "INVITATION_003",
            UserFriendlyMessage = "This invitation has already been accepted."
        },
        ["WORKSPACE_INVITATION_REVOKED"] = new()
        {
            Message = "Workspace invitation has been revoked.",
            StatusCode = 400,
            ErrorCode = "INVITATION_004",
            UserFriendlyMessage = "This invitation has been revoked by the sender."
        },
        ["WORKSPACE_INVITATION_EMAIL_REQUIRED"] = new()
        {
            Message = "Email address is required for invitation.",
            StatusCode = 400,
            ErrorCode = "INVITATION_005",
            UserFriendlyMessage = "Please provide a valid email address for the invitation."
        },
        ["WORKSPACE_INVITATION_INVALID_EMAIL"] = new()
        {
            Message = "Invalid email address for invitation.",
            StatusCode = 400,
            ErrorCode = "INVITATION_006",
            UserFriendlyMessage = "Please provide a valid email address."
        },
        ["WORKSPACE_INVITATION_SELF_INVITE"] = new()
        {
            Message = "Cannot invite yourself to workspace.",
            StatusCode = 400,
            ErrorCode = "INVITATION_007",
            UserFriendlyMessage = "You cannot invite yourself to a workspace you already own."
        },
        ["WORKSPACE_INVITATION_CREATE_FAILED"] = new()
        {
            Message = "Failed to create workspace invitation.",
            StatusCode = 500,
            ErrorCode = "INVITATION_008",
            UserFriendlyMessage = "We couldn't send the invitation. Please try again or contact support."
        },
        ["WORKSPACE_INVITATION_ACCEPT_FAILED"] = new()
        {
            Message = "Failed to accept workspace invitation.",
            StatusCode = 500,
            ErrorCode = "INVITATION_009",
            UserFriendlyMessage = "We couldn't accept the invitation. Please try again or contact support."
        },

        // Document errors
        ["DOCUMENT_NOT_FOUND"] = new()
        {
            Message = "Document not found.",
            StatusCode = 404,
            ErrorCode = "DOCUMENT_001",
            UserFriendlyMessage = "The document you're looking for doesn't exist or has been deleted."
        },
        ["DOCUMENT_ACCESS_DENIED"] = new()
        {
            Message = "You do not have access to this document.",
            StatusCode = 403,
            ErrorCode = "DOCUMENT_002",
            UserFriendlyMessage = "You don't have permission to access this document."
        },
        ["DOCUMENT_TITLE_REQUIRED"] = new()
        {
            Message = "Document title is required.",
            StatusCode = 400,
            ErrorCode = "DOCUMENT_003",
            UserFriendlyMessage = "Please provide a title for the document."
        },
        ["DOCUMENT_TITLE_TOO_LONG"] = new()
        {
            Message = "Document title is too long.",
            StatusCode = 400,
            ErrorCode = "DOCUMENT_004",
            UserFriendlyMessage = "Document title must be 256 characters or less."
        },
        ["DOCUMENT_YDOC_ID_REQUIRED"] = new()
        {
            Message = "Document YDoc ID is required.",
            StatusCode = 400,
            ErrorCode = "DOCUMENT_005",
            UserFriendlyMessage = "Document ID is required for real-time collaboration."
        },
        ["DOCUMENT_YDOC_ID_DUPLICATE"] = new()
        {
            Message = "Document YDoc ID already exists.",
            StatusCode = 409,
            ErrorCode = "DOCUMENT_006",
            UserFriendlyMessage = "A document with this ID already exists."
        },
        ["DOCUMENT_CREATE_FAILED"] = new()
        {
            Message = "Failed to create document.",
            StatusCode = 500,
            ErrorCode = "DOCUMENT_007",
            UserFriendlyMessage = "We couldn't create the document. Please try again or contact support."
        },
        ["DOCUMENT_UPDATE_FAILED"] = new()
        {
            Message = "Failed to update document.",
            StatusCode = 500,
            ErrorCode = "DOCUMENT_008",
            UserFriendlyMessage = "We couldn't update the document. Please try again or contact support."
        },
        ["DOCUMENT_DELETE_FAILED"] = new()
        {
            Message = "Failed to delete document.",
            StatusCode = 500,
            ErrorCode = "DOCUMENT_009",
            UserFriendlyMessage = "We couldn't delete the document. Please try again or contact support."
        },
        ["DOCUMENT_ARCHIVE_FAILED"] = new()
        {
            Message = "Failed to archive document.",
            StatusCode = 500,
            ErrorCode = "DOCUMENT_010",
            UserFriendlyMessage = "We couldn't archive the document. Please try again or contact support."
        },
        ["DOCUMENT_UNARCHIVE_FAILED"] = new()
        {
            Message = "Failed to unarchive document.",
            StatusCode = 500,
            ErrorCode = "DOCUMENT_011",
            UserFriendlyMessage = "We couldn't unarchive the document. Please try again or contact support."
        },

        // OTP errors
        ["OTP_NOT_FOUND"] = new()
        {
            Message = "OTP not found for this email.",
            StatusCode = 404,
            ErrorCode = "OTP_001",
            UserFriendlyMessage = "No OTP was found for this email. Please request a new OTP."
        },
        ["OTP_EXPIRED"] = new()
        {
            Message = "OTP has expired.",
            StatusCode = 400,
            ErrorCode = "OTP_002",
            UserFriendlyMessage = "Your OTP has expired. Please request a new one."
        },
        ["INVALID_OTP"] = new()
        {
            Message = "Invalid OTP code.",
            StatusCode = 400,
            ErrorCode = "OTP_003",
            UserFriendlyMessage = "The OTP you entered is incorrect. Please try again."
        },
        ["OTP_TOO_MANY_ATTEMPTS"] = new()
        {
            Message = "Too many invalid OTP attempts.",
            StatusCode = 429, // Too Many Requests
            ErrorCode = "OTP_004",
            UserFriendlyMessage = "You’ve entered the wrong OTP too many times. Please request a new one."
        },
        ["OTP_ALREADY_USED"] = new()
        {
            Message = "OTP has already been consumed.",
            StatusCode = 400,
            ErrorCode = "OTP_005",
            UserFriendlyMessage = "This OTP was already used. Please request a new one."
        },

        // JWT / Token errors
        ["TOKEN_MISSING"] = new()
        {
            Message = "Authorization token is missing.",
            StatusCode = 401,
            ErrorCode = "JWT_001",
            UserFriendlyMessage = "You must provide a valid token to access this resource."
        },
        ["TOKEN_INVALID"] = new()
        {
            Message = "Authorization token is invalid.",
            StatusCode = 401,
            ErrorCode = "JWT_002",
            UserFriendlyMessage = "Your login token is invalid. Please log in again."
        },
        ["TOKEN_EXPIRED"] = new()
        {
            Message = "Authorization token has expired.",
            StatusCode = 401,
            ErrorCode = "JWT_003",
            UserFriendlyMessage = "Your session has expired. Please log in again."
        },
        ["TOKEN_REVOKED"] = new()
        {
            Message = "Authorization token has been revoked.",
            StatusCode = 401,
            ErrorCode = "JWT_004",
            UserFriendlyMessage = "This token is no longer valid. Please log in again."
        },
        ["REFRESH_TOKEN_INVALID"] = new()
        {
            Message = "Refresh token is invalid.",
            StatusCode = 401,
            ErrorCode = "JWT_005",
            UserFriendlyMessage = "Your refresh token is invalid. Please log in again."
        },
        ["REFRESH_TOKEN_EXPIRED"] = new()
        {
            Message = "Refresh token has expired.",
            StatusCode = 401,
            ErrorCode = "JWT_006",
            UserFriendlyMessage = "Your refresh session has expired. Please log in again."
        },

        // Resource / generic errors
        ["RESOURCE_NOT_FOUND"] = new()
        {
            Message = "Requested resource was not found.",
            StatusCode = 404,
            ErrorCode = "GENERIC_001",
            UserFriendlyMessage = "The resource you’re looking for doesn’t exist."
        },
        ["INVALID_INPUT"] = new()
        {
            Message = "One or more inputs are invalid.",
            StatusCode = 400,
            ErrorCode = "GENERIC_002",
            UserFriendlyMessage = "Some of the data you entered is invalid. Please check and try again."
        },
        ["CONFLICT"] = new()
        {
            Message = "The request could not be completed due to a conflict.",
            StatusCode = 409,
            ErrorCode = "GENERIC_003",
            UserFriendlyMessage = "There’s a conflict with the current state of the resource."
        },
        ["RATE_LIMIT_EXCEEDED"] = new()
        {
            Message = "Rate limit exceeded.",
            StatusCode = 429,
            ErrorCode = "GENERIC_004",
            UserFriendlyMessage = "You’re sending too many requests. Please slow down and try again later."
        },
        ["SERVER_ERROR"] = new()
        {
            Message = "An internal server error occurred.",
            StatusCode = 500,
            ErrorCode = "SERVER_001",
            UserFriendlyMessage = "Something went wrong. Please try again later."
        },
        ["SERVICE_UNAVAILABLE"] = new()
        {
            Message = "The service is temporarily unavailable.",
            StatusCode = 503,
            ErrorCode = "SERVER_002",
            UserFriendlyMessage = "The system is under maintenance or overloaded. Please try again later."
        }
    };
}

public class ErrorInfo
{
    public required string Message { get; set; }
    public required int StatusCode { get; set; }
    public required string ErrorCode { get; set; }
    public required string UserFriendlyMessage { get; set; }
}
