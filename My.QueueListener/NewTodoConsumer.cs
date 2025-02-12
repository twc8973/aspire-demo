using Dapper;
using MassTransit;
using Microsoft.Data.SqlClient;
using My.Domain;

namespace My.QueueListener;

public class NewTodoConsumer : IConsumer<NewTodo>
{
    readonly ILogger<NewTodoConsumer> _logger;
    private readonly string _connectionString;

    public NewTodoConsumer(ILogger<NewTodoConsumer> logger, IConfiguration config)
    {
        _logger = logger;
        _connectionString = config.GetConnectionString("database")!;
    }

    public async Task Consume(ConsumeContext<NewTodo> context)
    {
        await Task.Delay(1000);
        _logger.LogInformation("Received Text: {Text}", context.Message.Title);

        var connection = new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            @"UPDATE Todos   
SET Random = @random
WHERE TItle = @title;",
            new
            {
                title = context.Message.Title,
                random = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
            });
    }
}