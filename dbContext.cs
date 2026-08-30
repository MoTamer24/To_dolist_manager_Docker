using Microsoft.EntityFrameworkCore;

namespace Todo_App;

// EF Core database context.
// This is the place we will later swap to SQL Server by changing the provider,
// while keeping the rest of the application code the same.
public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskTodo> Tasks => Set<TaskTodo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskTodo>(entity =>
        {
            entity.HasKey(task => task.Id);
            entity.Property(task => task.TaskName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(task => task.Description)
                .HasMaxLength(1000);

            entity.Property(task => task.CreatedAt)
                .IsRequired();
        });
    }
}
