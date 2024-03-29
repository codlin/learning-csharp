// using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebApp.Filters;
using WebApp.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opts =>
{
    opts.UseNpgsql(@$"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};Username=test;Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWD")};Database=Products");
    opts.EnableSensitiveDataLogging(true);
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<GuidResponseAttribute>();
// builder.Services.Configure<MvcOptions>(opts =>
// {
//     opts.Filters.Add<HttpsOnlyAttribute>();
//     opts.Filters.Add(new MessageAttribute("This is the globally-scoped filter"));
// });

var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.MapControllerRoute("forms", "controllers/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedDatabase(context);
app.Run();
