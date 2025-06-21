using O2OBackend.Application.DTOs.Menu;
using O2OBackend.Domain.Entities;
using O2OBackend.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // <-- 新增：引入日誌命名空間

namespace O2OBackend.Application.Services.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly ILogger<MenuService> _logger; // <-- 新增：聲明 ILogger 欄位

        // <-- 修改：在建構子中注入 ILogger
        public MenuService(IMenuRepository menuRepository, ILogger<MenuService> logger)
        {
            _menuRepository = menuRepository;
            _logger = logger; // <-- 賦值
        }

        public async Task<IEnumerable<MenuDto>> GetAllMenusAsync()
        {
            _logger.LogInformation("Retrieving all menus."); // 日誌：開始獲取所有選單
            var menus = await _menuRepository.GetAllAsync();
            _logger.LogInformation("Successfully retrieved {MenuCount} menus.", menus.Count()); // 日誌：獲取完成
            return menus.Select(m => new MenuDto
            {
                Id = m.Id,
                ParentId = m.ParentId,
                Name = m.Name,
                Icon = m.Icon,
                Path = m.Path,
                ComponentPath = m.ComponentPath,
                SortOrder = m.SortOrder,
                PermissionCode = m.PermissionCode,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            });
        }

        public async Task<MenuDto?> GetMenuByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve menu with ID: {MenuId}", id); // 日誌：嘗試獲取選單
            var menu = await _menuRepository.GetByIdAsync(id);
            if (menu == null)
            {
                _logger.LogWarning("Menu with ID: {MenuId} not found.", id); // 日誌：選單未找到
                return null;
            }
            _logger.LogInformation("Successfully retrieved menu with ID: {MenuId}.", id); // 日誌：獲取成功
            return new MenuDto
            {
                Id = menu.Id,
                ParentId = menu.ParentId,
                Name = menu.Name,
                Icon = menu.Icon,
                Path = menu.Path,
                ComponentPath = menu.ComponentPath,
                SortOrder = menu.SortOrder,
                PermissionCode = menu.PermissionCode,
                IsActive = menu.IsActive,
                CreatedAt = menu.CreatedAt,
                UpdatedAt = menu.UpdatedAt
            };
        }

        public async Task<int> CreateMenuAsync(MenuCreateDto menuCreateDto)
        {
            _logger.LogInformation("Attempting to create new menu: {MenuName}", menuCreateDto.Name); // 日誌：嘗試創建選單
            if (menuCreateDto.ParentId.HasValue)
            {
                var parentMenu = await _menuRepository.GetByIdAsync(menuCreateDto.ParentId.Value);
                if (parentMenu == null || !parentMenu.IsActive)
                {
                    _logger.LogWarning("Menu creation failed: Parent menu with ID {ParentId} not found or is inactive.", menuCreateDto.ParentId.Value); // 日誌：父選單無效
                    throw new ApplicationException($"Parent menu with ID {menuCreateDto.ParentId.Value} not found or is inactive.");
                }
            }

            var menu = new Menu
            {
                ParentId = menuCreateDto.ParentId,
                Name = menuCreateDto.Name,
                Icon = menuCreateDto.Icon,
                Path = menuCreateDto.Path,
                ComponentPath = menuCreateDto.ComponentPath,
                SortOrder = menuCreateDto.SortOrder,
                PermissionCode = menuCreateDto.PermissionCode,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var newMenuId = await _menuRepository.AddAsync(menu);
            _logger.LogInformation("Successfully created menu with ID: {MenuId} and name: {MenuName}", newMenuId, menuCreateDto.Name); // 日誌：創建成功
            return newMenuId;
        }

        public async Task<bool> UpdateMenuAsync(MenuUpdateDto menuUpdateDto)
        {
            _logger.LogInformation("Attempting to update menu with ID: {MenuId}", menuUpdateDto.Id); // 日誌：嘗試更新選單
            var menu = await _menuRepository.GetByIdAsync(menuUpdateDto.Id);
            if (menu == null)
            {
                _logger.LogWarning("Menu update failed: Menu with ID {MenuId} not found.", menuUpdateDto.Id); // 日誌：選單不存在
                return false;
            }

            if (menuUpdateDto.ParentId.HasValue && menu.ParentId != menuUpdateDto.ParentId)
            {
                var parentMenu = await _menuRepository.GetByIdAsync(menuUpdateDto.ParentId.Value);
                if (parentMenu == null || !parentMenu.IsActive)
                {
                    _logger.LogWarning("Menu update failed: Parent menu with ID {ParentId} not found or is inactive.", menuUpdateDto.ParentId.Value); // 日誌：父選單無效
                    throw new ApplicationException($"Parent menu with ID {menuUpdateDto.ParentId.Value} not found or is inactive.");
                }
            }
            if (menuUpdateDto.ParentId.HasValue && menuUpdateDto.ParentId.Value == menuUpdateDto.Id)
            {
                _logger.LogWarning("Menu update failed: A menu cannot be its own parent. Menu ID: {MenuId}.", menuUpdateDto.Id); // 日誌：父選單不能是自己
                 throw new ApplicationException("A menu cannot be its own parent.");
            }

            menu.ParentId = menuUpdateDto.ParentId;
            menu.Name = menuUpdateDto.Name;
            menu.Icon = menuUpdateDto.Icon;
            menu.Path = menuUpdateDto.Path;
            menu.ComponentPath = menuUpdateDto.ComponentPath;
            menu.SortOrder = menuUpdateDto.SortOrder;
            menu.PermissionCode = menuUpdateDto.PermissionCode;
            menu.IsActive = menuUpdateDto.IsActive;
            menu.UpdatedAt = DateTime.Now;

            var updated = await _menuRepository.UpdateAsync(menu);
            if (updated)
            {
                _logger.LogInformation("Successfully updated menu with ID: {MenuId}.", menuUpdateDto.Id); // 日誌：更新成功
            }
            else
            {
                _logger.LogError("Failed to update menu with ID: {MenuId} in repository.", menuUpdateDto.Id); // 日誌：更新失敗 (Repository 層)
            }
            return updated;
        }

        public async Task<bool> DeleteMenuAsync(int id)
        {
            _logger.LogInformation("Attempting to delete menu with ID: {MenuId}", id); // 日誌：嘗試刪除選單
            var deleted = await _menuRepository.DeleteAsync(id);
            if (deleted)
            {
                _logger.LogInformation("Successfully deleted menu with ID: {MenuId}.", id); // 日誌：刪除成功
            }
            else
            {
                _logger.LogWarning("Menu deletion failed: Menu with ID {MenuId} not found or could not be deleted.", id); // 日誌：刪除失敗
            }
            return deleted;
        }

        public async Task<IEnumerable<MenuDto>> GetAccessibleMenusAsync(IEnumerable<string>? permissionCodes)
        {
            _logger.LogInformation("Retrieving accessible menus for permission codes: {PermissionCodes}", permissionCodes != null ? string.Join(", ", permissionCodes) : "None"); // 日誌：獲取可訪問選單
            var menus = await _menuRepository.GetAccessibleMenusByPermissionCodesAsync(permissionCodes ?? Enumerable.Empty<string>());
            _logger.LogInformation("Successfully retrieved {MenuCount} accessible menus.", menus.Count()); // 日誌：獲取完成
            return menus.Select(m => new MenuDto
            {
                Id = m.Id,
                ParentId = m.ParentId,
                Name = m.Name,
                Icon = m.Icon,
                Path = m.Path,
                ComponentPath = m.ComponentPath,
                SortOrder = m.SortOrder,
                PermissionCode = m.PermissionCode,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            });
        }
    }
}