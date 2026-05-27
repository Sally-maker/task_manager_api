using TaskManagerApi.Constants;
using TaskManagerApi.DTOs;
using TaskManagerApi.Models;
using TaskManagerApi.Repositories;

namespace TaskManagerApi.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TaskResponseDto>> GetAllAsync(string? status)
    {
        var tasks = await _repository.GetAllAsync(status);
        return tasks.Select(MapToResponse);
    }

    public async Task<TaskResponseDto?> GetByIdAsync(int id)
    {
        var task = await _repository.GetByIdAsync(id);
        return task is null ? null : MapToResponse(task);
    }

    public async Task<TaskResponseDto> CreateAsync(CreateTaskDto dto)
    {
        var now = DateTime.UtcNow;

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = TaskStatuses.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };

        var created = await _repository.CreateAsync(task);
        return MapToResponse(created);
    }

    public async Task<(bool found, bool conflict)> UpdateAsync(int id, UpdateTaskDto dto)
    {
        var task = await _repository.GetByIdAsync(id);

        if (task is null)
            return (found: false, conflict: false);

        if (task.Status == TaskStatuses.Done)
            return (found: true, conflict: true);

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(task);
        return (found: true, conflict: false);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _repository.GetByIdAsync(id);

        if (task is null)
            return false;

        await _repository.DeleteAsync(task);
        return true;
    }

    private static TaskResponseDto MapToResponse(TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status,
        CreatedAt = task.CreatedAt,
        UpdatedAt = task.UpdatedAt
    };
}
