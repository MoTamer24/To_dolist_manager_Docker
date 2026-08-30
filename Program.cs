using Todo_App;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Logging is added here because this app is meant to be easy to debug while testing.
// In a real app, you can keep this, or move to a more structured logging provider.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Dependency injection for the application.
// For testing now, we use EF Core's in-memory database so there is no SQL Server setup required.
// When you are ready for SQL Server, switch the provider to UseSqlServer and update the connection string.
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseInMemoryDatabase("TodoAppDb"));

builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

app.MapGet("/", () => "Todo API is running. Use /tasks to create, list, update, toggle and delete tasks.");

app.MapGet("/tasks", async (ITaskService taskService) =>
{
    var result = await taskService.GetAllAsync();
    return result.Success ? Results.Ok(result.Data) : Results.BadRequest(result.Message);
});

app.MapGet("/tasks/{id:guid}", async (Guid id, ITaskService taskService) =>
{
    var result = await taskService.GetByIdAsync(id);
    return result.Success ? Results.Ok(result.Data) : Results.NotFound(result.Message);
});

app.MapPost("/tasks", async (TaskCreateRequest request, ITaskService taskService) =>
{
    var result = await taskService.CreateAsync(request);
    return result.Success ? Results.Ok(result.Data) : Results.BadRequest(result.Message);
});

app.MapPut("/tasks/{id:guid}", async (Guid id, TaskUpdateRequest request, ITaskService taskService) =>
{
    var result = await taskService.UpdateAsync(id, request);
    return result.Success ? Results.Ok(result.Data) : Results.NotFound(result.Message);
});

app.MapPatch("/tasks/{id:guid}/toggle", async (Guid id, ITaskService taskService) =>
{
    var result = await taskService.ToggleDoneAsync(id);
    return result.Success ? Results.Ok(result.Data) : Results.NotFound(result.Message);
});

app.MapDelete("/tasks/{id:guid}", async (Guid id, ITaskService taskService) =>
{
    var result = await taskService.DeleteAsync(id);
    return result.Success ? Results.Ok(result.Message ?? "Task deleted.") : Results.NotFound(result.Message);
});

app.Run();
