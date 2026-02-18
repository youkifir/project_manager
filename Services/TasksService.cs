using Microsoft.EntityFrameworkCore;
using project_manager.Data;
using project_manager.Models.DTOs;
using project_manager.Models.Entities;

namespace project_manager.Services
{
    public interface IServiceTask
    {
        Task<List<TaskDto>> GetTasksByProjectAsync(int projectId, string userId, string? sortOrder = null);
        Task<ProjectTask?> GetByIdAsync(int taskId, string userId);
        Task<ProjectTask?> CreateAsync(ProjectTask task, string userId);
        Task<ProjectTask?> UpdateAsync(int taskId, ProjectTask model, string userId);
        Task<bool> DeleteAsync(int taskId, string userId);
    }

    public class TasksService : IServiceTask
    {
        private readonly ApplicationDbContext _db;

        public TasksService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TaskDto>> GetTasksByProjectAsync(int projectId, string userId, string? sortOrder = null)
        {
            var query = _db.ProjectTasks
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId && t.Project.OwnerId == userId);

            query = sortOrder switch
            {
                "A-Z" => query.OrderBy(t => t.Title),
                "Z-A" => query.OrderByDescending(t => t.Title),
                "date_asc" => query.OrderBy(t => t.DueDate),
                "date_desc" => query.OrderByDescending(t => t.DueDate),
                _ => query.OrderBy(t => t.TaskId)
            };

            return await query
                .Select(t => new TaskDto(
                    t.TaskId,
                    t.Title,
                    t.Description,
                    t.DueDate,
                    t.IsCompleted,
                    t.AssignedUser != null ? t.AssignedUser.UserName : "Не назначен"
                ))
                .ToListAsync();
        }

        public async Task<ProjectTask?> GetByIdAsync(int taskId, string userId)
        {
            return await _db.ProjectTasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.TaskId == taskId && t.Project.OwnerId == userId);
        }

        public async Task<ProjectTask?> CreateAsync(ProjectTask task, string userId)
        {
            var project = await _db.Projects
                .AnyAsync(p => p.ProjectId == task.ProjectId && p.OwnerId == userId);

            if (!project) return null;

            if (string.IsNullOrEmpty(task.AssignedUserId))
            {
                task.AssignedUserId = userId;
            }

            await _db.ProjectTasks.AddAsync(task);
            await _db.SaveChangesAsync();
            return task;
        }

        public async Task<ProjectTask?> UpdateAsync(int taskId, ProjectTask model, string userId)
        {
            var existingTask = await _db.ProjectTasks
                .FirstOrDefaultAsync(t => t.TaskId == taskId && t.Project.OwnerId == userId);

            if (existingTask == null) return null;

            existingTask.Title = model.Title;
            existingTask.Description = model.Description;
            existingTask.DueDate = model.DueDate;
            existingTask.IsCompleted = model.IsCompleted;
            existingTask.AssignedUserId = model.AssignedUserId;

            await _db.SaveChangesAsync();
            return existingTask;
        }

        public async Task<bool> DeleteAsync(int taskId, string userId)
        {
            var task = await _db.ProjectTasks
                .FirstOrDefaultAsync(t => t.TaskId == taskId && t.Project.OwnerId == userId);

            if (task == null) return false;

            _db.ProjectTasks.Remove(task);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}