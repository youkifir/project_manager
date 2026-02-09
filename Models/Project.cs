using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_manager.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdateAt { get; set; }
        [Required]
        public string OwnerId { get; set; } = string.Empty;
        [ForeignKey("OwnerId")]
        public ApplicationUser? Owner { get; set; }
        public ICollection<Task>? Tasks { get; set; }
    }
}
