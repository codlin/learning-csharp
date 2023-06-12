using ExampleApp;
using ExampleApp.Custom;

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

app.UseMiddleware<CustomAuthentication>();
app.UseMiddleware<RoleMemberships>();
app.UseRouting();
app.UseMiddleware<ClaimsReporter>();
app.UseMiddleware<CustomAuthorization>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
    endpoints.MapGet("/secret", SecretEndpoint.Endpoint).WithDisplayName("secret");
    endpoints.Map("/signin", CustomSignInAndSignOut.SignIn);
    endpoints.Map("/signout", CustomSignInAndSignOut.SignOut);
});

// app.MapGet("/", async context =>
// {
//     await context.Response.WriteAsync("Hello World!");
// });
// app.MapGet("/secret", SecretEndpoint.Endpoint).WithDisplayName("secret");

app.Run();
