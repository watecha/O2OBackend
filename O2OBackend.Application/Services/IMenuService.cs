using O2OBackend.Application.DTOs.Menu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace O2OBackend.Application.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuDto>> GetAllMenusAsync();
        Task<MenuDto?> GetMenuByIdAsync(int id);
        Task<int> CreateMenuAsync(MenuCreateDto menuCreateDto);
        Task<bool> UpdateMenuAsync(MenuUpdateDto menuUpdateDto);
        Task<bool> DeleteMenuAsync(int id);
        Task<IEnumerable<MenuDto>> GetAccessibleMenusAsync(IEnumerable<string>? permissionCodes);
    }
}