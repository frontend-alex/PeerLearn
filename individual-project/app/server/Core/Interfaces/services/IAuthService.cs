namespace Core.Interfaces.Services;

public interface IAuthService {
    Task<string> RegisterAsync(string username, string firstName, string lastName, string email, string password);
    Task<string> LoginAsync(string email, string password);
}
