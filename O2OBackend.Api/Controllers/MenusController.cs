using Microsoft.AspNetCore.Mvc;
using O2OBackend.Application.DTOs.Menu;
using O2OBackend.Application.Services; // 引入 IMenuService
using System.Collections.Generic;
using System.Threading.Tasks;

namespace O2OBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _menuService; // 使用應用程式服務介面

        public MenusController(IMenuService menuService) // 注入 IMenuService
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuDto>>> Get()
        {
            var menus = await _menuService.GetAllMenusAsync();
            return Ok(menus);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuDto>> Get(int id)
        {
            var menu = await _menuService.GetMenuByIdAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            return Ok(menu);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] MenuCreateDto menuCreateDto)
        {
            try
            {
                var newMenuId = await _menuService.CreateMenuAsync(menuCreateDto);
                return StatusCode(201, newMenuId);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] MenuUpdateDto menuUpdateDto)
        {
            if (id != menuUpdateDto.Id)
            {
                return BadRequest("Menu ID mismatch.");
            }
            try
            {
                var updated = await _menuService.UpdateMenuAsync(menuUpdateDto);
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
            var deleted = await _menuService.DeleteMenuAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("accessible")] // 使用 POST 方法，因為請求體中可能包含權限碼列表
        public async Task<ActionResult<IEnumerable<MenuDto>>> GetAccessibleMenus([FromBody] IEnumerable<string>? permissionCodes)
        {
            try
            {
                var menus = await _menuService.GetAccessibleMenusAsync(permissionCodes);
                return Ok(menus);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}