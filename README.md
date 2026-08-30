# Todo App API

A simple ASP.NET Core Todo manager built for easy testing and later migration to SQL Server.

## What this app does

- Create tasks
- List all tasks
- Get one task by ID
- Update task name and description
- Toggle task done/not done
- Delete tasks

## Current database setup

This project is currently using an EF Core in-memory database so you can test the API quickly without setting up SQL Server.

This is intentionally simple and is meant for development and testing.

## Project structure

- `Program.cs` - API setup and routes
- `task.cs` - task model
- `taskHandler.cs` - business logic and request models
- `dbContext.cs` - EF Core database context
- `appsettings.json` - app config

## Run the app

Open a terminal in the project folder:

```bash
cd "Multi-Container Application/Todo_App"
dotnet run
```

The app will start on:

```text
http://localhost:5050
```

## API endpoints

### Get all tasks

```http
GET /tasks
```

### Get one task by ID

```http
GET /tasks/{id}
```

### Create a task

```http
POST /tasks
```

Example JSON body:

```json
{
  "taskName": "Buy milk",
  "description": "Get 2 liters"
}
```

### Update a task

```http
PUT /tasks/{id}
```

Example JSON body:

```json
{
  "taskName": "Buy milk and bread",
  "description": "Get 2 liters and 1 loaf"
}
```

### Toggle task done state

```http
PATCH /tasks/{id}/toggle
```

### Delete a task

```http
DELETE /tasks/{id}
```

## Example command-line testing with curl

### Create a task

```bash
curl -X POST "http://localhost:5050/tasks" \
  -H "Content-Type: application/json" \
  -d '{"taskName":"Buy milk","description":"Need 2 liters"}'
```

### Get all tasks

```bash
curl http://localhost:5050/tasks
```

### Toggle task state

```bash
curl -X PATCH "http://localhost:5050/tasks/{id}/toggle"
```

### Delete task

```bash
curl -X DELETE "http://localhost:5050/tasks/{id}"
```

## Switch to SQL Server later

When you are ready, install the SQL Server EF provider and update the database configuration in `Program.cs`.

Example package:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

Then update the DB registration in `Program.cs` from in-memory to SQL Server, for example:

```csharp
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

And add a connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TodoDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Notes

- This app is intentionally simple and beginner-friendly.
- Logging is included to help with debugging.
- The structure is already close to a real EF Core setup, so moving to SQL Server later should be easy.

## Troubleshooting

### App won't start

Check that you are in the project folder and run:

```bash
dotnet restore
dotnet build
dotnet run
```

### Port issue

If port 5050 is already used, you can start with a different URL:

```bash
dotnet run --urls http://localhost:5001
```
