using O2OBackend.Application.DTOs.User;
using O2OBackend.Domain.Entities;
using O2OBackend.Domain.Repositories;
using O2OBackend.Shared.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // <-- 新增：引入日誌命名空間

namespace O2OBackend.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<UserService> _logger; // <-- 新增：聲明 ILogger 欄位

        // <-- 修改：在建構子中注入 ILogger
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _logger = logger; // <-- 賦值
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            _logger.LogInformation("Retrieving all users."); // 日誌：開始獲取所有用戶
            var users = await _userRepository.GetAllAsync();
            _logger.LogInformation("Successfully retrieved {UserCount} users.", users.Count()); // 日誌：獲取完成
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve user with ID: {UserId}", id); // 日誌：嘗試獲取用戶
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID: {UserId} not found.", id); // 日誌：用戶未找到
                return null;
            }
            _logger.LogInformation("Successfully retrieved user with ID: {UserId}.", id); // 日誌：獲取成功
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            _logger.LogInformation("Attempting to retrieve user with username: {Username}", username);
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("User with username: {Username} not found.", username);
                return null;
            }
            _logger.LogInformation("Successfully retrieved user with username: {Username}.", username);
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<int> CreateUserAsync(UserCreateDto userCreateDto)
        {
            _logger.LogInformation("Attempting to create new user: {Username}", userCreateDto.Username); // 日誌：嘗試創建用戶
            var existingUser = await _userRepository.GetByUsernameAsync(userCreateDto.Username);
            if (existingUser != null)
            {
                _logger.LogWarning("User creation failed: Username '{Username}' already exists.", userCreateDto.Username); // 日誌：用戶名重複
                throw new ApplicationException($"Username '{userCreateDto.Username}' already exists.");
            }

            var passwordHash = PasswordHasher.HashPassword(userCreateDto.Password);

            var user = new User
            {
                Username = userCreateDto.Username,
                PasswordHash = passwordHash,
                Email = userCreateDto.Email,
                FullName = userCreateDto.FullName,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var newUserId = await _userRepository.AddAsync(user);
            _logger.LogInformation("Successfully created user with ID: {UserId} and username: {Username}", newUserId, userCreateDto.Username); // 日誌：創建成功
            return newUserId;
        }

        public async Task<bool> UpdateUserAsync(UserUpdateDto userUpdateDto)
        {
            _logger.LogInformation("Attempting to update user with ID: {UserId}", userUpdateDto.Id); // 日誌：嘗試更新用戶
            var user = await _userRepository.GetByIdAsync(userUpdateDto.Id);
            if (user == null)
            {
                _logger.LogWarning("User update failed: User with ID {UserId} not found.", userUpdateDto.Id); // 日誌：用戶不存在
                return false;
            }

            if (user.Username != userUpdateDto.Username)
            {
                var existingUserWithNewUsername = await _userRepository.GetByUsernameAsync(userUpdateDto.Username);
                if (existingUserWithNewUsername != null && existingUserWithNewUsername.Id != userUpdateDto.Id)
                {
                    _logger.LogWarning("User update failed: Username '{Username}' is already taken by another user (ID: {ExistingUserId}).", userUpdateDto.Username, existingUserWithNewUsername.Id); // 日誌：用戶名衝突
                    throw new ApplicationException($"Username '{userUpdateDto.Username}' is already taken by another user.");
                }
            }

            user.Username = userUpdateDto.Username;
            user.Email = userUpdateDto.Email;
            user.FullName = userUpdateDto.FullName;
            user.IsActive = userUpdateDto.IsActive;
            user.UpdatedAt = DateTime.Now;

            var updated = await _userRepository.UpdateAsync(user);
            if (updated)
            {
                _logger.LogInformation("Successfully updated user with ID: {UserId}.", userUpdateDto.Id); // 日誌：更新成功
            }
            else
            {
                _logger.LogError("Failed to update user with ID: {UserId} in repository.", userUpdateDto.Id); // 日誌：更新失敗 (Repository 層)
            }
            return updated;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            _logger.LogInformation("Attempting to delete user with ID: {UserId}", id); // 日誌：嘗試刪除用戶
            var deleted = await _userRepository.DeleteAsync(id);
            if (deleted)
            {
                _logger.LogInformation("Successfully deleted user with ID: {UserId}.", id); // 日誌：刪除成功
            }
            else
            {
                _logger.LogWarning("User deletion failed: User with ID {UserId} not found or could not be deleted.", id); // 日誌：刪除失敗
            }
            return deleted;
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
        {
            _logger.LogInformation("Attempting to assign role ID: {RoleId} to user ID: {UserId}.", roleId, userId); // 日誌：嘗試分配角色
            var userExists = await _userRepository.GetByIdAsync(userId) != null;
            var roleExists = await _roleRepository.GetByIdAsync(roleId) != null;

            if (!userExists)
            {
                _logger.LogWarning("Assign role failed: User with ID {UserId} not found.", userId); // 日誌：用戶不存在
                throw new ApplicationException($"User with ID {userId} not found.");
            }
            if (!roleExists)
            {
                _logger.LogWarning("Assign role failed: Role with ID {RoleId} not found.", roleId); // 日誌：角色不存在
                throw new ApplicationException($"Role with ID {roleId} not found.");
            }

            var assigned = await _roleRepository.AssignRoleToUserAsync(userId, roleId);
            if (assigned)
            {
                _logger.LogInformation("Successfully assigned role ID: {RoleId} to user ID: {UserId}.", roleId, userId); // 日誌：分配成功
            }
            else
            {
                _logger.LogError("Failed to assign role ID: {RoleId} to user ID: {UserId}.", roleId, userId); // 日誌：分配失敗
            }
            return assigned;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            _logger.LogInformation("Attempting to remove role ID: {RoleId} from user ID: {UserId}.", roleId, userId); // 日誌：嘗試移除角色
            var userExists = await _userRepository.GetByIdAsync(userId) != null;
            var roleExists = await _roleRepository.GetByIdAsync(roleId) != null;

            if (!userExists)
            {
                _logger.LogWarning("Remove role failed: User with ID {UserId} not found.", userId);
                throw new ApplicationException($"User with ID {userId} not found.");
            }
            if (!roleExists)
            {
                _logger.LogWarning("Remove role failed: Role with ID {RoleId} not found.", roleId);
                throw new ApplicationException($"Role with ID {roleId} not found.");
            }

            var removed = await _roleRepository.RemoveRoleFromUserAsync(userId, roleId);
            if (removed)
            {
                _logger.LogInformation("Successfully removed role ID: {RoleId} from user ID: {UserId}.", roleId, userId); // 日誌：移除成功
            }
            else
            {
                _logger.LogError("Failed to remove role ID: {RoleId} from user ID: {UserId}.", roleId, userId); // 日誌：移除失敗
            }
            return removed;
        }
    }
}