namespace Core.Models;

public class Otp : BaseEntity {
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime ExpirationTime { get; set; }
} 