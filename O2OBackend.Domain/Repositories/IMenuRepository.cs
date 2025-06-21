using O2OBackend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace O2OBackend.Domain.Repositories
{
    public interface IMenuRepository
    {
        Task<Menu?> GetByIdAsync(int id);
        Task<IEnumerable<Menu>> GetAllAsync();
        Task<int> AddAsync(Menu menu);
        Task<bool> UpdateAsync(Menu menu);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Menu>> GetAccessibleMenusByPermissionCodesAsync(IEnumerable<string> permissionCodes);
    }
}