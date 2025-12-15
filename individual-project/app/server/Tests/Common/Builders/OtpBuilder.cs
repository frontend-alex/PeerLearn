using Core.Models;

namespace PeerLearn.Tests.Common.Builders;

/// <summary>
/// Builder pattern for creating test OTP entities
/// </summary>
public class OtpBuilder
{
    private Otp _otp;

    public OtpBuilder()
    {
        _otp = new Otp
        {
            Email = $"test_{Guid.NewGuid():N}@example.com",
            Code = "123456",
            ExpirationTime = DateTime.UtcNow.AddMinutes(5)
        };
    }

    public OtpBuilder WithId(int id)
    {
        _otp.Id = id;
        return this;
    }

    public OtpBuilder WithEmail(string email)
    {
        _otp.Email = email;
        return this;
    }

    public OtpBuilder WithCode(string code)
    {
        _otp.Code = code;
        return this;
    }

    public OtpBuilder WithExpirationTime(DateTime expirationTime)
    {
        _otp.ExpirationTime = expirationTime;
        return this;
    }

    public OtpBuilder WithExpiredTime()
    {
        _otp.ExpirationTime = DateTime.UtcNow.AddMinutes(-1);
        return this;
    }

    public Otp Build() => _otp;
}

