using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// SQL Server container is configured with an auto-generated password by default
// but doesn't support any auto-creation of databases or running scripts on startup so we have to do it manually.
var sqlserver = builder.AddSqlServer("sqlserver")
    // Mount the init scripts directory into the container.
    .WithBindMount("./sqlserverconfig", "/usr/config")
    // Mount the SQL scripts directory into the container so that the init scripts run.
    .WithBindMount("./sql", "/docker-entrypoint-initdb.d")
    // Run the custom entrypoint script on startup.
    //.WithEntrypoint("/usr/config/entrypoint.sh")
    .WithArgs("/usr/config/entrypoint.sh")
    // Configure the container to store data in a volume so that it persists across instances.
    //.WithDataVolume()
    // Keep the container running between app host sessions.
    .WithLifetime(ContainerLifetime.Session);

var database = sqlserver.AddDatabase("database", "MyDemo");

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin();

builder.AddProject<Projects.My_Api>("api")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(database)
    .WaitFor(database);

builder.AddProject<Projects.My_QueueListener>("queuelistener")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();