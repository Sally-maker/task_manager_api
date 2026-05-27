namespace TaskManagerApi.DTOs;

public class UpdateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
}
