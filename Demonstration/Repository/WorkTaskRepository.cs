using Demonstration.Data;
using Demonstration.Migrations;
using Demonstration.Models.Entities;
using Demonstration.Repository.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demonstration.Repository
{
    public class WorkTaskRepository : IWorkTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTaskAsync(WorkTask task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAssignedEmployeeAsync(int taskId)
        {
            var assignedIds = await _context.UserTasks
                .Where(x => x.TaskId == taskId)
                .Select(x => x.UserId)
                .ToListAsync();

            return await _context.Users
                .Where(u => assignedIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<AssignResult> RemoveEmployeeFromTask(Guid userId, int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                return new AssignResult { Status = Models.AssignStatus.TaskNotFound };

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new AssignResult { Status = Models.AssignStatus.UserNotFound };

            var assignedEmployee = await _context.UserTasks.FirstOrDefaultAsync(ut => ut.TaskId == taskId && ut.UserId == userId);

            if (assignedEmployee == null)
                return new AssignResult { Status = Models.AssignStatus.Failed };

            _context.UserTasks.Remove(assignedEmployee);
            task.EnrolledParticipants--;
            await _context.SaveChangesAsync();

            return new AssignResult { Status = Models.AssignStatus.Sucess };
        }

        public async Task<AssignResult> AssignUserToTaskAsync(Guid userId, int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                return new AssignResult { Status = Models.AssignStatus.TaskNotFound };

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new AssignResult { Status = Models.AssignStatus.UserNotFound };

            var alreadyAssigned = await _context.UserTasks
                .AnyAsync(x => x.TaskId == taskId && x.UserId == userId);

            if (alreadyAssigned || task.EnrolledParticipants >= task.MaximumParticipants)
                return new AssignResult { Status = Models.AssignStatus.ReachMax };

            _context.UserTasks.Add(new Models.Entities.EmployeeTask
            {
                TaskId = taskId,
                UserId = userId
            });

            task.EnrolledParticipants++;
            await _context.SaveChangesAsync();

            return new AssignResult
            {
                Status = Models.AssignStatus.Sucess,
                TaskName = task.Name,
                UserEmail = user.Email
            };
        }

        public async Task DeleteTaskAsync(WorkTask task)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WorkTask>> GetAllTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<WorkTask?> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<List<User>> GetUnassignedEmployeesAsync(int taskId)
        {
            var assignedIds = await _context.UserTasks
                .Where(x => x.TaskId == taskId)
                .Select(x => x.UserId)
                .ToListAsync();

            return await _context.Users
                .Where(u => !assignedIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task UpdateTaskAsync(WorkTask task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }
    }
}
