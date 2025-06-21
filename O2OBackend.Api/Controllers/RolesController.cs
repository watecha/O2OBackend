using Microsoft.AspNetCore.Mvc;
using O2OBackend.Application.DTOs.Role;
using O2OBackend.Application.DTOs.Permission;
using O2OBackend.Application.DTOs.User;
using O2OBackend.Application.Services; // 引入 IRoleService
using System.Collections.Generic;
using System.Threading.Tasks;

namespace O2OBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService; // 使用應用程式服務介面
        private readonly IUserService _userService; // 因為 User 對 Role 的操作現在在 UserService 中

        public RolesController(IRoleService roleService, IUserService userService) // 注入 IRoleService 和 IUserService
        {
            _roleService = roleService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> Get()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> Get(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        [HttpGet("name/{name}")] // 新增一個根據名稱查詢的端點
        public async Task<ActionResult<RoleDto>> GetByName(string name)
        {
            var role = await _roleService.GetRoleByNameAsync(name);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] RoleCreateDto roleCreateDto)
        {
            try
            {
                var newRoleId = await _roleService.CreateRoleAsync(roleCreateDto);
                return StatusCode(201, newRoleId);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RoleUpdateDto roleUpdateDto)
        {
            if (id != roleUpdateDto.Id)
            {
                return BadRequest("Role ID mismatch.");
            }
            try
            {
                var updated = await _roleService.UpdateRoleAsync(roleUpdateDto);
                if (!updated)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _roleService.DeleteRoleAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        // 關聯操作現在通過各自的服務來協調
        [HttpPost("{roleId}/assign-permission/{permissionId}")]
        public async Task<IActionResult> AssignPermissionToRole(int roleId, int permissionId)
        {
            try
            {
                var assigned = await _roleService.AssignPermissionToRoleAsync(roleId, permissionId);
                if (!assigned)
                {
                    return BadRequest("Failed to assign permission to role.");
                }
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{roleId}/remove-permission/{permissionId}")]
        public async Task<IActionResult> RemovePermissionFromRole(int roleId, int permissionId)
        {
            try
            {
                var removed = await _roleService.RemovePermissionFromRoleAsync(roleId, permissionId);
                if (!removed)
                {
                    return BadRequest("Failed to remove permission from role.");
                }
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{roleId}/permissions")]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissionsByRole(int roleId)
        {
            var permissions = await _roleService.GetPermissionsForRoleAsync(roleId);
            return Ok(permissions);
        }

        [HttpGet("{roleId}/users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(int roleId)
        {
            var users = await _roleService.GetUsersInRoleAsync(roleId);
            return Ok(users);
        }
    }
}