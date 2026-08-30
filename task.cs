namespace Todo_App;

// This is the main entity stored in the database.
// Naming it in PascalCase keeps it consistent with C# conventions and makes it easier
// to use with EF Core and JSON serialization.
public class TaskTodo
{
    public Guid Id { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Done { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
