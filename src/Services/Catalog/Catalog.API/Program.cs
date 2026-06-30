var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR(config => // Register MediatR services, which allows you to implement the mediator pattern for handling requests and responses.
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly); // Register MediatR handlers from the current assembly
});
builder.Services.AddCarter(); // Add Carter services to the DI container, which allows you to define endpoints using Carter modules.
builder.Services.AddSingleton(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("Database") // Retrieve the connection string for the database from the configuration
    ?? throw new InvalidOperationException("Connection string 'Database' is not configured"); // and throw an exception if it's not found
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString); // Create a new NpgsqlDataSourceBuilder using the connection string
    return dataSourceBuilder.Build(); // Build and return the NpgsqlDataSource, which is used to manage database connections
});
builder.Services.AddScoped<CatalogInitialData>(); // Register the CatalogInitialData as a scoped service, which means a new instance will be created for each HTTP request
builder.Services.AddScoped<CatalogRepository>(); // Register the CatalogRepository as a scoped service, which allows you to manage product data in the database


var app = builder.Build();

// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var initialData = scope.ServiceProvider.GetRequiredService<CatalogInitialData>();

    await initialData.InitializeAsync(
        seedSampleData: app.Environment.IsDevelopment()
    );
}
app.MapCarter(); // Map Carter endpoints

app.Run();
