var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddDbContext<DataContext>(opts =>
// {
//     opts.UseNpgsql(@$"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};Username=test;Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWD")};Database=Products");
//     opts.EnableSensitiveDataLogging(true);
// });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.MapRazorPages();
app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Hello World!");
});
app.Run();
