using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var db=new dbContext();
var handler=new taskHandler(db);

app.MapGet("/", () => "Hello World!");
app.MapPost("/create", (taskDTOCreate dto) =>
{
    var status=handler.create(dto);
    return status;
});

app.MapGet("/all", () =>
{
    return handler.all();
});

app.Run();
