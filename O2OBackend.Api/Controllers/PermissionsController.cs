using Microsoft.AspNetCore.Mvc;
using O2OBackend.Application.DTOs.Permission;
using O2OBackend.Application.Services; // 引入 IPermissionService
using System.Collections.Generic;
using System.Threading.Tasks;

namespace O2OBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService; // 使用應用程式服務介面

        public PermissionsController(IPermissionService permissionService) // 注入 IPermissionService
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> Get()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            return Ok(permissions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDto>> Get(int id)
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            if (permission == null)
            {
                return NotFound();
            }
            return Ok(permission);
        }

        [HttpGet("code/{code}")] // 新增根據 Code 查詢
        public async Task<ActionResult<PermissionDto>> GetByCode(string code)
        {
            var permission = await _permissionService.GetPermissionByCodeAsync(code);
            if (permission == null)
            {
                return NotFound();
            }
            return Ok(permission);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] PermissionCreateDto permissionCreateDto)
        {
            try
            {
                var newPermissionId = await _permissionService.CreatePermissionAsync(permissionCreateDto);
                return StatusCode(201, newPermissionId);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PermissionUpdateDto permissionUpdateDto)
        {
            if (id != permissionUpdateDto.Id)
            {
                return BadRequest("Permission ID mismatch.");
            }
            try
            {
                var updated = await _permissionService.UpdatePermissionAsync(permissionUpdateDto);
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
            var deleted = await _permissionService.DeletePermissionAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("user/{userId}")] // 獲取用戶所有權限
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissionsForUser(int userId)
        {
            try
            {
                var permissions = await _permissionService.GetPermissionsForUserAsync(userId);
                return Ok(permissions);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}