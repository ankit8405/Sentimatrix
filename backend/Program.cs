using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SentimatrixAPI.Services;
using SentimatrixAPI.Data;
using Microsoft.OpenApi.Models;
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

// Add MongoDB Settings
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

builder.Services.AddSingleton<EmailService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sentimatrix API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapHub<TicketHub>("/ticketHub");

// Ensure we're using HTTP
app.Urls.Clear();
app.Urls.Add("http://localhost:5000");

app.Run();
