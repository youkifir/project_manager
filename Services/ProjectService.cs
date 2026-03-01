using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using project_manager.Data;
using project_manager.Models.DTOs;
using project_manager.Models.Entities;

namespace project_manager.Services
{
    public interface IServiceProject
    {
        Task<List<Project>> GetUserProjectsAsync(string userId, string? searchTerm = null);
        Task<Project?> GetByIdAsync(int projectId, string userId);
        Task<Project> CreateAsync(Project project, string userId);
        Task<Project?> UpdateAsync(int projectId, ProjectDto model, string userId);
        Task<Project?> DeleteAsync(int projectId, string userId);

        Task<Project?> GetByIdInternalAsync(int projectId);
    }
    public class ProjectService : IServiceProject
    {
        private readonly ApplicationDbContext _db;

        public ProjectService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId, string? searchTerm = null)
        {
            var query = _db.Projects
                .AsNoTracking()
                .Where(p => p.OwnerId == userId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            return await query.ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int projectId, string userId)
        {
            return await _db.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.OwnerId == userId);
        }

        public async Task<Project> CreateAsync(Project project, string userId)
        {
            project.OwnerId = userId;
            project.CreatedAt = DateTime.UtcNow;
            project.UpdateAt = DateTime.UtcNow;

            await _db.Projects.AddAsync(project);
            await _db.SaveChangesAsync();
            return project;
        }

        public async Task<Project?> UpdateAsync(int projectId, ProjectDto model, string userId)
        {
            var existingProject = await _db.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.OwnerId == userId);

            if (existingProject == null) return null;

            existingProject.Name = model.Name;
            existingProject.Description = model.Description;
            existingProject.UpdateAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return existingProject;
        }

        public async Task<Project?> DeleteAsync(int projectId, string userId)
        {
            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == projectId && p.OwnerId == userId);

            if (project == null) return null;

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
            return project;
        }

        public async Task<Project?> GetByIdInternalAsync(int projectId)
        {
            return await _db.Projects.FindAsync(projectId);
        }
    }
}
