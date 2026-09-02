using Todo_App;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Logging is added here because this app is meant to be easy to debug while testing.
// In a real app, you can keep this, or move to a more structured logging provider.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var cs = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"[DEBUG] Connection string: {cs}");

// Dependency injection for the application.
// Configured to use SQL Server based on the connection string in appsettings.json.
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    db.Database.Migrate(); // if using migrations
    // or: db.Database.EnsureCreated(); // if not using migrations at all
}

app.MapGet("/", () => $"[DEBUG] Connection string: {cs}");

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
