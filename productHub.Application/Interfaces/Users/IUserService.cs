using productHub.Application.DTOs.Users;

namespace productHub.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto> CreateAsync(UserDto user);      // para admins
    Task<bool> UpdateAsync(Guid id, UserDto user);
    Task<bool> DeleteAsync(Guid id);
}