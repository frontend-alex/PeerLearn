using Core.Models;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.SQL;

namespace Infrastructure.Repositories;

public class OtpRepository : IOtpRepository {
    private readonly ApplicationDbContext _context;

    public OtpRepository(ApplicationDbContext context) {
        _context = context;
    }

    public async Task CreateOtpAsync(Otp otp) {
        _context.Otps.Add(otp);
        await _context.SaveChangesAsync();
    }

    public async Task<Otp?> GetOtpByEmailAsync(string email) {
        return await _context.Otps
            .FirstOrDefaultAsync(o => o.Email == email);
    }

    public async Task DeleteOtpByEmailAsync(string email) {
        var otp = await _context.Otps
            .FirstOrDefaultAsync(o => o.Email == email);

        if (otp != null) {
            _context.Otps.Remove(otp);
            await _context.SaveChangesAsync();
        }
    }
}
