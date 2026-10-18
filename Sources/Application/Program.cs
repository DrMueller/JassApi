using System.Text.Json.Serialization;
using JassApi.Application;
using JassApi.Authentication;
using JassApi.Infrastructure;
using JassApi.Mcp;
using JassApi.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IJassApiDbContextFactory>(_ =>
{
    var connectionString = builder.Configuration["AppSettings:ConnectionString"]
                           ?? throw new InvalidOperationException("AppSettings:ConnectionString must be configured.");
    return new JassApiDbContextFactory(JassApiDbContextOptions.Create(connectionString));
});
builder.Services.AddScoped<IJassDataQueryService, JassDataQueryService>();
builder.Services.AddEntraIdApiAuthentication(builder.Configuration);

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options => options.Stateless = true)
    .AddAuthorizationFilters()
    .WithTools<PlayerDataTools>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapGet("/health", () => TypedResults.Ok(new { status = "healthy" }))
    .AllowAnonymous()
    .WithName("Health")
    .WithSummary("Check API health")
    .Produces(StatusCodes.Status200OK);

app.MapMcp("/mcp").RequireAuthorization(JassApiAuthorization.PolicyName);

app.Run();

public partial class Program;
