using productHub.Application.DTOs.Users;
using productHub.Application.Interfaces;
using productHub.Application.Security;
using productHub.Domain.Entities;
using productHub.Domain.Interfaces;

namespace productHub.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Password = "",
            Role = u.Role
        });
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Password = "",
            Role = user.Role
        };
    }

    public async Task<UserDto> CreateAsync(UserDto dto)
    {
        var hashedPassword = _passwordHasher.Hash(dto.Password);

        var user = new User(dto.Username, dto.Email, hashedPassword, dto.Role);
        await _userRepository.AddAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Password = "",
            Role = user.Role
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UserDto dto)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing is null) return false;

        var newPassword = string.IsNullOrWhiteSpace(dto.Password)
            ? existing.Password
            : _passwordHasher.Hash(dto.Password);

        var updated = new User(dto.Username, dto.Email, newPassword, dto.Role);

        typeof(User).GetProperty("Id")!.SetValue(updated, id);

        await _userRepository.UpdateAsync(updated);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);
        return true;
    }
}
