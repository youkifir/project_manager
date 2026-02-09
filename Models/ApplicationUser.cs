using Microsoft.AspNetCore.Identity;

namespace project_manager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Project>? OwnedProjects { get; set; }
        public ICollection<Task>? AssignedTasks { get; set; }
    }
}
