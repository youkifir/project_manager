using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_manager.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DueDate { get; set; }
        [Required]
        public bool IsCompleted { get; set; } = false;
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public string AssignedUserId { get; set; } = string.Empty;
        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }
        [ForeignKey("AssignedUserId")]
        public ApplicationUser? AssignedUser { get; set; }
    }
}