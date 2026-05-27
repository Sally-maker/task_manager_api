using TaskManagerApi.DTOs;

namespace TaskManagerApi.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetAllAsync(string? status);
    Task<TaskResponseDto?> GetByIdAsync(int id);
    Task<TaskResponseDto> CreateAsync(CreateTaskDto dto);
    Task<(bool found, bool conflict)> UpdateAsync(int id, UpdateTaskDto dto);
    Task<bool> DeleteAsync(int id);
}
