using Microsoft.EntityFrameworkCore;
using SportsStore.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<StoreDbContext>(opts => {
    opts.UseNpgsql(@$"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};Username=test;Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWD")};Database=SportsStore");
});
builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();

var app = builder.Build();
app.UseStaticFiles();
app.MapControllerRoute("pagination", "Products/Page{productPage}", new { Controller = "Home", action = "Index" });
app.MapDefaultControllerRoute();

SeedData.EnsurePopulated(app);

app.Run();
