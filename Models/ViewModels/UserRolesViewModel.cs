using project_manager.Models.Entities;

namespace project_manager.Models.ViewModels
{
    public class UserRolesViewModel
    {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}
