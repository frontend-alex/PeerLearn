namespace Core.Services.User;

using System;
using System.Linq;
using Core.Models;
using Core.Exceptions;
using Infrastructure.Repositories;
using Core.DTOs;
using Core.Mappers;
using Core.Interfaces.Services;

public class UserService : IUserService {
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository) {
        _userRepository = userRepository;
    }

    public async Task<UserDto> GetByIdUser(int id) {
        User user = await _userRepository.GetByIdAsync(id)
            ?? throw AppException.CreateError("USER_NOT_FOUND");

        return UserMapper.ToUserDto(user);
    }

    public async Task<IEnumerable<UserDto>> SearchUsers(string query, int limit) {
        if (string.IsNullOrWhiteSpace(query)) {
            return Enumerable.Empty<UserDto>();
        }

        int cappedLimit = Math.Clamp(limit, 1, 25);

        IEnumerable<User> users = await _userRepository.SearchAsync(query, cappedLimit);

        return users.Select(UserMapper.ToUserDto);
    }

    public async Task UpdateUser(int id, Dictionary<string, object> updates) {
        User? user = await _userRepository.GetByIdAsync(id)
            ?? throw AppException.CreateError("USER_NOT_FOUND");

        foreach (var update in updates) {
            var property = typeof(User).GetProperty(update.Key);
            if (property != null && property.CanWrite) {
                property.SetValue(user, update.Value);
            }
        }

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUser(int id) {
        User user = await _userRepository.GetByIdAsync(id)
            ?? throw AppException.CreateError("USER_NOT_FOUND");

        bool deleted = await _userRepository.DeleteAsync(id);

        if (!deleted) {
            throw AppException.CreateError("USER_DELETE_FAILED");
        }
    }
}
