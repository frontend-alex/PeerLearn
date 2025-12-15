using Core.Exceptions;
using Core.Models;
using Core.Utils.Email;
using Infrastructure.Repositories;

namespace Core.Services.Auth;

using Core.Interfaces.Services;

public class OtpService : IOtpService {
    private readonly IOtpRepository _otpRepository;
    private readonly IUserRepository _userRepository;
    private readonly TimeSpan _otpValidityDuration = TimeSpan.FromMinutes(5);

    public OtpService(IOtpRepository otpRepository, IUserRepository userRepository) {
        _otpRepository = otpRepository;
        _userRepository = userRepository;
    }

    public async Task<DateTime> SendOtpAsync(string email) {

        if (await _userRepository.GetByEmailAsync(email) == null) {
            throw AppException.CreateError("USER_NOT_FOUND");
        }

        var otpCode = OtpUtils.GenerateOtpCode();
        var expirationTime = DateTime.UtcNow.Add(_otpValidityDuration);

        // Delete any existing OTP for this email
        await _otpRepository.DeleteOtpByEmailAsync(email);

        // Create new OTP
        var otp = new Otp {
            Email = email,
            Code = otpCode,
            ExpirationTime = expirationTime
        };

        await _otpRepository.CreateOtpAsync(otp);

        return expirationTime;
    }

    public async Task VerifyOtpAsync(string email, string code) {

        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
            throw AppException.CreateError("USER_NOT_FOUND");

        if (user.EmailVerified)
            throw AppException.CreateError("USER_ALREADY_VERIFIED");

        var otp = await _otpRepository.GetOtpByEmailAsync(email);

        if (otp is null)
            throw AppException.CreateError("OTP_NOT_FOUND");

        if (otp.ExpirationTime < DateTime.UtcNow) {
            await _otpRepository.DeleteOtpByEmailAsync(email);
            throw AppException.CreateError("OTP_EXPIRED");
        }

        if (otp.Code != code)
            throw AppException.CreateError("INVALID_OTP");

        await _otpRepository.DeleteOtpByEmailAsync(email);

        user.EmailVerified = true;
        await _userRepository.UpdateAsync(user);
    }
}
