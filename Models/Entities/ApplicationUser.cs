using Microsoft.AspNetCore.Identity;

namespace project_manager.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Project>? OwnedProjects { get; set; }
        public ICollection<ProjectTask>? AssignedTasks { get; set; }
    }
}
