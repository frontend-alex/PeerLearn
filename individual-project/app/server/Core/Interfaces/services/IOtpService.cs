namespace Core.Interfaces.Services;

public interface IOtpService {
    Task<DateTime> SendOtpAsync(string email);
    Task VerifyOtpAsync(string email, string code);
}

