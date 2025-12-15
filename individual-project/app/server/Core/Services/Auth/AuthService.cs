namespace Core.Services.Auth;

using Core.Models;
using Core.Exceptions;
using Infrastructure.Repositories;
using Core.Interfaces.Services;

public class AuthService : IAuthService {
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IPasswordService passwordService, IJwtService jwtService) {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<string> RegisterAsync(string username, string firstName, string lastName, string email, string password) {

        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser != null) {
            if (!existingUser.EmailVerified) {
                throw AppException.CreateError("EMAIL_EXISTS_NOT_VERIFIED", extra: new Dictionary<string, object> { { "email", email } });
            }
            throw AppException.CreateError("EMAIL_EXISTS");
        }

        if (await _userRepository.ExistsByUsernameAsync(username)) {
            throw AppException.CreateError("USERNAME_EXISTS");
        }

        var user = new User {
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = _passwordService.HashPassword(password),
            EmailVerified = false,
            Xp = 0
        };

        await _userRepository.CreateAsync(user);

        return user.Email;
    }


    public async Task<string> LoginAsync(string email, string password) {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null || !_passwordService.VerifyPassword(password, user.PasswordHash)) {
            throw AppException.CreateError("INVALID_CREDENTIALS");
        }

        if (!user.EmailVerified) {
            throw AppException.CreateError("EMAIL_NOT_VERIFIED", extra: new Dictionary<string, object> { { "email", email }, { "otpRedirect", true } });
        }

        var token = _jwtService.GenerateToken(user);

        return token;
    }
}
