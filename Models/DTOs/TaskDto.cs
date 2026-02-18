namespace project_manager.Models.DTOs
{
    public record TaskDto(
        int Id,
        string Title,
        string? Description,
        DateTime DueDate,
        bool IsCompleted,
        string AssignedUserName
    );
}
