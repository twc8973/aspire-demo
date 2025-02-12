using Dapper;
using MassTransit;
using Microsoft.Data.SqlClient;
using My.Domain;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("messaging");
        cfg.Host(connectionString);
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/all"));

app.MapGet("/new", async (IConfiguration config, IBus bus) =>
{
    var connection = new SqlConnection(config.GetConnectionString("database"));

    var title = Guid.NewGuid().ToString().Substring(0, 8);
    await connection.ExecuteAsync(
        @"INSERT INTO Todos (Title, Description)
VALUES (@title, @description);",
        new
        {
            title = title,
            description = Guid.NewGuid().ToString(),
        });

    await bus.Publish(new NewTodo(title));

    return Results.Redirect("/all");
});

app.MapGet("/all", async (IConfiguration config) =>
{
    var connection = new SqlConnection(config.GetConnectionString("database"));

    return await connection.QueryAsync<Todo>(@"select Id, Title, Description, Random from Todos ORDER BY ID DESC");
});

app.Run();

public record Todo(int Id, string Title, string Description, string Random);