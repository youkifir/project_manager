using Microsoft.EntityFrameworkCore;
using project_manager.Data;
using project_manager.Models;

namespace project_manager.Services
{
    public interface IServiceProject
    {
        public Task<Project> CreateAsync(Project project, string userId);
        public Task<IEnumerable<Project>> GetAllAsync();
        public Task<Project> GetByIdAsync(int id);
        public Task<Project> UpdateAsync(int id, Project project);
        public System.Threading.Tasks.Task DeleteAsync(int id);
        public Task<List<Project>> GetUserProjectsAsync(string userId);
    }
    public class ServiceProject : IServiceProject
    {
        private readonly ApplicationDbContext _db;
        public ServiceProject(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Project> CreateAsync(Project project, string userId)
        {
            project.OwnerId = userId;

            await _db.AddAsync(project);
            await _db.SaveChangesAsync();
            return project;
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var project = await GetByIdAsync(id);
            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Project>> GetAllAsync() => await _db.Projects.ToListAsync();

        public async Task<Project> GetByIdAsync(int id)
        {
            var project = await _db.Projects.FindAsync(id);
            if(project == null)
            {
                throw new KeyNotFoundException($"Project with id {id} not found.");
            }
            return project;
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            return await _db.Projects
            .Where(p => p.OwnerId == userId)
            .ToListAsync();
        }

        public async Task<Project> UpdateAsync(int id, Project project)
        {
            var project_for_update = await GetByIdAsync(id);

            project_for_update.Name = project.Name;
            project_for_update.Description = project.Description;
            project_for_update.UpdateAt = DateTime.Now;

            await _db.SaveChangesAsync();
            return project_for_update;
        }
    }
}
