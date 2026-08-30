using Microsoft.EntityFrameworkCore;

namespace Todo_App;

// Simple service contract for the business logic.
// The route layer depends on this abstraction, not the concrete EF context directly.
public interface ITaskService
{
    Task<OperationResult<List<TaskTodo>>> GetAllAsync();
    Task<OperationResult<TaskTodo>> GetByIdAsync(Guid id);
    Task<OperationResult<TaskTodo>> CreateAsync(TaskCreateRequest request);
    Task<OperationResult<TaskTodo>> UpdateAsync(Guid id, TaskUpdateRequest request);
    Task<OperationResult<TaskTodo>> ToggleDoneAsync(Guid id);
    Task<OperationResult> DeleteAsync(Guid id);
}

public class TaskService : ITaskService
{
    private readonly TodoDbContext _db;
    private readonly ILogger<TaskService> _logger;

    public TaskService(TodoDbContext db, ILogger<TaskService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<OperationResult<List<TaskTodo>>> GetAllAsync()
    {
        var tasks = await _db.Tasks
            .OrderBy(task => task.CreatedAt)
            .ToListAsync();

        _logger.LogInformation("Fetched {TaskCount} tasks from the database.", tasks.Count);
        return OperationResult<List<TaskTodo>>.Ok(tasks, "Tasks retrieved successfully.");
    }

    public async Task<OperationResult<TaskTodo>> GetByIdAsync(Guid id)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(item => item.Id == id);

        if (task is null)
        {
            _logger.LogWarning("Task with ID {TaskId} was not found.", id);
            return OperationResult<TaskTodo>.Fail("Task not found.");
        }

        return OperationResult<TaskTodo>.Ok(task, "Task retrieved successfully.");
    }

    public async Task<OperationResult<TaskTodo>> CreateAsync(TaskCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TaskName))
        {
            _logger.LogWarning("Task creation failed because the name is empty.");
            return OperationResult<TaskTodo>.Fail("Task name is required.");
        }

        var task = new TaskTodo
        {
            Id = Guid.NewGuid(),
            TaskName = request.TaskName.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Done = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        try
        {
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Created task {TaskId} with name {TaskName}.",
                task.Id,
                task.TaskName);

            return OperationResult<TaskTodo>.Ok(task, "Task created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create a task.");
            return OperationResult<TaskTodo>.Fail("Database error while creating the task.");
        }
    }

    public async Task<OperationResult<TaskTodo>> UpdateAsync(Guid id, TaskUpdateRequest request)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(item => item.Id == id);

        if (task is null)
        {
            _logger.LogWarning("Update failed because task {TaskId} was not found.", id);
            return OperationResult<TaskTodo>.Fail("Task not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.TaskName))
        {
            task.TaskName = request.TaskName.Trim();
        }

        if (request.Description is not null)
        {
            task.Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();
        }

        task.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _db.SaveChangesAsync();
            _logger.LogInformation("Updated task {TaskId}.", task.Id);
            return OperationResult<TaskTodo>.Ok(task, "Task updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update task {TaskId}.", id);
            return OperationResult<TaskTodo>.Fail("Database error while updating the task.");
        }
    }

    public async Task<OperationResult<TaskTodo>> ToggleDoneAsync(Guid id)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(item => item.Id == id);

        if (task is null)
        {
            _logger.LogWarning("Toggle failed because task {TaskId} was not found.", id);
            return OperationResult<TaskTodo>.Fail("Task not found.");
        }

        task.Done = !task.Done;
        task.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _db.SaveChangesAsync();
            _logger.LogInformation("Toggled task {TaskId} to Done={DoneValue}.", task.Id, task.Done);
            return OperationResult<TaskTodo>.Ok(task, "Task state changed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle task {TaskId}.", id);
            return OperationResult<TaskTodo>.Fail("Database error while toggling the task.");
        }
    }

    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(item => item.Id == id);

        if (task is null)
        {
            _logger.LogWarning("Delete failed because task {TaskId} was not found.", id);
            return OperationResult.Fail("Task not found.");
        }

        try
        {
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Deleted task {TaskId}.", id);
            return OperationResult.Ok("Task deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete task {TaskId}.", id);
            return OperationResult.Fail("Database error while deleting the task.");
        }
    }
}

public record OperationResult(bool Success, string? Message = null)
{
    public static OperationResult Ok(string? message = null) => new(true, message);
    public static OperationResult Fail(string message) => new(false, message);
}

public record OperationResult<T>(bool Success, T? Data, string? Message = null)
{
    public static OperationResult<T> Ok(T data, string? message = null) => new(true, data, message);
    public static OperationResult<T> Fail(string message) => new(false, default, message);
}

public class TaskCreateRequest
{
    public string TaskName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class TaskUpdateRequest
{
    public string? TaskName { get; set; }
    public string? Description { get; set; }
}

