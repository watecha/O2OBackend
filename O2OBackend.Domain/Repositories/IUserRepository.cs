using O2OBackend.Domain.Entities; // 假設 User 實體在這裡

namespace O2OBackend.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();
        Task<int> AddAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        // 您可以根據需要添加其他更複雜的查詢方法
    }
}