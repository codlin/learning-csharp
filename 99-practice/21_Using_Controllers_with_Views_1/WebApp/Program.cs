using Microsoft.EntityFrameworkCore;
using WebApp.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opts =>
{
    opts.UseNpgsql(@$"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};Username=test;Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWD")};Database=Products");
    opts.EnableSensitiveDataLogging(true);
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<CitiesData>();

var app = builder.Build();
app.UseStaticFiles();
app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedDatabase(context);

app.Run();