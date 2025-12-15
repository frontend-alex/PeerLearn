namespace Core.Interfaces.Services;

using Core.DTOs;

public interface IUserService {
    Task<UserDto> GetByIdUser(int id);
    Task<IEnumerable<UserDto>> SearchUsers(string query, int limit);
    Task UpdateUser(int id, Dictionary<string, object> updates);
    Task DeleteUser(int id);
}

