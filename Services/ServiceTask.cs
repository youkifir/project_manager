using Microsoft.EntityFrameworkCore;
using project_manager.Data;
using project_manager.Models;

namespace project_manager.Services
{
    public interface IServiceTask
    {
        public Task<Models.Task> CreateAsync(Models.Task task);
        public Task<IEnumerable<Models.Task>> GetAllAsync();
        public Task<Models.Task> GetByIdAsync(int id);
        public Task<Models.Task> UpdateAsync(int id, Models.Task task);
        public System.Threading.Tasks.Task DeleteAsync(int id);
    }

    public class ServiceTask : IServiceTask
    {
        private readonly ApplicationDbContext _db;

        public ServiceTask(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Models.Task> CreateAsync(Models.Task task)
        {
            await _db.Tasks.AddAsync(task);
            await _db.SaveChangesAsync();
            return task;
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var task = await GetByIdAsync(id);
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Models.Task>> GetAllAsync()
            => await _db.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .ToListAsync();

        public async Task<Models.Task> GetByIdAsync(int id)
        {
            var task = await _db.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .FirstOrDefaultAsync(t => t.TaskId == id);

            if (task == null)
            {
                throw new KeyNotFoundException($"Task with id {id} not found.");
            }
            return task;
        }

        public async Task<Models.Task> UpdateAsync(int id, Models.Task task)
        {
            var task_for_update = await GetByIdAsync(id);

            task_for_update.Title = task.Title;
            task_for_update.Description = task.Description;
            task_for_update.DueDate = task.DueDate;
            task_for_update.IsCompleted = task.IsCompleted;

            await _db.SaveChangesAsync();
            return task_for_update;
        }
    }
}