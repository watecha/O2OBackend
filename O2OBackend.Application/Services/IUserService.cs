using O2OBackend.Application.DTOs.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace O2OBackend.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<int> CreateUserAsync(UserCreateDto userCreateDto);
        Task<bool> UpdateUserAsync(UserUpdateDto userUpdateDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> AssignRoleToUserAsync(int userId, int roleId);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
    }
}