using Orders.API.ExceptionHandlers;
using Orders.API.Services;
using Orders.API.Data;
using Orders.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<OrdersService>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddExceptionHandler<OrderValidationExceptionHandler>();
builder.Services.AddExceptionHandler<OrderNotFoundExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<DatabaseInitializer>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var initializer =
        scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

    initializer.Initialize();
}
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
