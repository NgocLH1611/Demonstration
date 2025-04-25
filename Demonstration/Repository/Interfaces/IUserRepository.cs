using Demonstration.Models.Entities;

namespace Demonstration.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<List<Role>> GetAllRolesAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        Task DetailAsync(Guid id);
        Task<List<WorkTask>> GetWorkTasksAsync(Guid id);
    }
}
