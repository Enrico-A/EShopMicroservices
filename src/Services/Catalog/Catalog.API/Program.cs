var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCarter(); // Add Carter services to the DI container, which allows you to define endpoints using Carter modules.
builder.Services.AddMediatR(config => // Register MediatR services, which allows you to implement the mediator pattern for handling requests and responses.
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly); // Register MediatR handlers from the current assembly
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapCarter(); // Map Carter endpoints

app.Run();
