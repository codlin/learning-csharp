using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(opts =>
{
    opts.UseNpgsql(@$"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};Username=test;Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWD")};Database=Products");
    opts.EnableSensitiveDataLogging(true);
});

// the AddControllers method defines the services that are required by the MVC framework.
builder.Services.AddControllers();
builder.Services.Configure<JsonOptions>(opts => {
opts.JsonSerializerOptions.DefaultIgnoreCondition
= JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

// the MapControllers method defines routes that will allow controllers to handle requests.
app.MapControllers();

app.UseMiddleware<WebApp.TestMiddleware>();

app.MapGet("/", () => "Hello World!");

var context = app.Services.CreateScope().ServiceProvider
    .GetRequiredService<DataContext>();
SeedData.SeedDatabase(context);

app.Run();
