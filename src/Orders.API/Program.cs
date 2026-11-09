using Orders.API.ExceptionHandlers;
using Orders.API.Services;
using Orders.API.Data;
using Orders.API.Middleware;
using Microsoft.AspNetCore.Mvc;
using Orders.API.Exceptions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Orders.API.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Orders.API.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.AddAppLogging();

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    
    options.InvalidModelStateResponseFactory = context =>
    {
        var http = context.HttpContext;
        http.Items["errorCode"] = OrderValidationException.ErrorCode;
        const string mensaje = "Los datos de la orden son inválidos.";

        var problema = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "Bad Request",
            Status = 400,
            Detail = mensaje,
            Instance = http.Request.Path.Value
        };

        problema.Extensions["errorCode"] = OrderValidationException.ErrorCode;
        problema.Extensions["errorMessage"] = mensaje;
        problema.Extensions["correlationId"] = http.TraceIdentifier;

        var respuesta = new BadRequestObjectResult(problema);
        respuesta.ContentTypes.Add("application/problem+json");

        return respuesta;
    };
});

builder.Services.AddTransient<IClientErrorFactory, ClientErrorFactory>();
builder.Services.AddScoped<OrdersService>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddExceptionHandler<OrderValidationExceptionHandler>();
builder.Services.AddExceptionHandler<OrderNotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<InsufficientStockExceptionHandler>();
builder.Services.AddExceptionHandler<OrderStatusConflictExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddHttpClient<UsersClient>(client =>
{
    string direccion = builder.Configuration["Services:Users"]
        ?? throw new InvalidOperationException("Falta configurar Services:Users.");

    client.BaseAddress = new Uri(direccion);
    client.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddHttpClient<ProductsClient>(client =>
{
    string direccion = builder.Configuration["Services:Products"]
        ?? throw new InvalidOperationException("Falta configurar Services:Products.");

    client.BaseAddress = new Uri(direccion);
    client.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddHealthChecks()
    .AddCheck<OrdersDatabaseHealthCheck>("sqlite-db", tags: ["ready"])
    .AddCheck("api-status",
        () => HealthCheckResult.Healthy("Orders.API está operativa."),
        tags: ["live"]);

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var initializer =
        scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

    initializer.Initialize();
}
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseStatusCodePages(async statusContext =>
{
    var http = statusContext.HttpContext;
    int status = http.Response.StatusCode;

    if (status != 404 && status != 405)
    {
        return;
    }

    string codigo = status == 404
        ? OrderNotFoundException.ErrorCode
        : OrderValidationException.ErrorCode;
    
    http.Items["errorCode"] = codigo;
    string mensaje = status == 404
        ? "La ruta solicitada no existe."
        : "El método HTTP no está permitido para esta ruta.";

    await Results.Problem(
        statusCode: status,
        detail: mensaje,
        instance: http.Request.Path.Value,
        extensions: new Dictionary<string, object?>
        {
            ["errorCode"] = codigo,
            ["errorMessage"] = mensaje,
            ["correlationId"] = http.TraceIdentifier
        }).ExecuteAsync(http);
});
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
