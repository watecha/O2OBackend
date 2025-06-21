using Microsoft.AspNetCore.Mvc;
using O2OBackend.Application.DTOs.User; // 引入 DTOs
using O2OBackend.Application.Services; // 引入 IUserService
using System.Collections.Generic;
using System.Threading.Tasks;

namespace O2OBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService; // 使用應用程式服務介面

        public UsersController(IUserService userService) // 注入 IUserService
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get() // 返回 UserDto
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(int id) // 返回 UserDto
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("username/{username}")] // 新增一個根據用戶名查詢的端點
        public async Task<ActionResult<UserDto>> GetByUsername(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }


        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] UserCreateDto userCreateDto) // 接收 UserCreateDto
        {
            try
            {
                var newUserId = await _userService.CreateUserAsync(userCreateDto);
                return StatusCode(201, newUserId); // 返回 201 Created 並帶上新用戶 ID
            }
            catch (ApplicationException ex) // 捕獲服務層拋出的業務異常
            {
                return BadRequest(ex.Message); // 返回 400 Bad Request
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserUpdateDto userUpdateDto) // 接收 UserUpdateDto
        {
            if (id != userUpdateDto.Id)
            {
                return BadRequest("User ID mismatch.");
            }
            try
            {
                var updated = await _userService.UpdateUserAsync(userUpdateDto);
                if (!updated)
                {
                    return NotFound(); // 用戶不存在
                }
                return NoContent(); // 204 No Content for successful update
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{userId}/assign-role/{roleId}")]
        public async Task<IActionResult> AssignRoleToUser(int userId, int roleId)
        {
            try
            {
                var assigned = await _userService.AssignRoleToUserAsync(userId, roleId);
                if (!assigned)
                {
                    return BadRequest("Failed to assign role to user."); // 服務層會處理是否存在，這裡處理通用失敗
                }
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{userId}/remove-role/{roleId}")]
        public async Task<IActionResult> RemoveRoleFromUser(int userId, int roleId)
        {
            try
            {
                var removed = await _userService.RemoveRoleFromUserAsync(userId, roleId);
                if (!removed)
                {
                    return BadRequest("Failed to remove role from user.");
                }
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}