using Demonstration.Models.Entities;

namespace Demonstration.Repository.Interfaces
{
    public interface IWorkTaskRepository
    {
        Task<List<WorkTask>> GetAllTasksAsync();
        Task<WorkTask?> GetTaskByIdAsync(int id);
        Task<User?> GetUserByIdAsync(Guid id);
        Task AddTaskAsync(WorkTask task);
        Task UpdateTaskAsync(WorkTask task);
        Task DeleteTaskAsync(WorkTask task);
        Task<List<User>> GetUnassignedEmployeesAsync(int taskId);
        Task<AssignResult> AssignUserToTaskAsync(Guid userId, int taskId);
        Task<List<User>> GetAssignedEmployeeAsync(int taskId);
        Task<AssignResult> RemoveEmployeeFromTask(Guid userId, int taskId);
    }
}
