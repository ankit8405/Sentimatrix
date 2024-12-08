using Microsoft.OpenApi.Models;
using SentimatrixAPI.Services;
using SentimatrixAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Sentimatrix API", 
        Version = "v1",
        Description = "API for processing emails and analyzing sentiment"
    });
});
builder.Services.AddSignalR();

// Register GroqService
builder.Services.AddSingleton<GroqService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyHeader()
               .AllowAnyMethod()
               .SetIsOriginAllowed((host) => true)
               .AllowCredentials();
    });
});

var app = builder.Build();

// Enable Swagger for all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sentimatrix API V1");
    c.RoutePrefix = "swagger";
});

// Use CORS before routing
app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapHub<TicketHub>("/ticketHub");

// Ensure we're using HTTP
app.Urls.Clear();
app.Urls.Add("http://localhost:5000");

app.Run();
