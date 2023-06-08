using System.Text;
using IdentityApp;
using IdentityApp.Models;
using IdentityApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityDbContextConnection' not found.");
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ProductDbContext>(opts =>
{
    opts.UseNpgsql(@$"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};Username=test;Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWD")};Database=IdentityAppData");
});

builder.Services.AddDbContext<IdentityDbContext>(opts =>
{
    string conStr = @$"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};Username=test;Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWD")};Database=IdentityAppUserData";
    opts.UseNpgsql(conStr, opts => opts.MigrationsAssembly("IdentityApp"));
});
builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(opts =>
{
    opts.Password.RequiredLength = 8;
    opts.Password.RequireDigit = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireNonAlphanumeric = false;
    opts.SignIn.RequireConfirmedAccount = true;
}).AddEntityFrameworkStores<IdentityDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<SecurityStampValidatorOptions>(opts =>
{
    opts.ValidationInterval = System.TimeSpan.FromMinutes(1);
});

builder.Services.AddScoped<TokenUrlEncoderService>();
builder.Services.AddScoped<IdentityEmailService>();

builder.Services.AddAuthentication().AddGoogle(opts =>
{
    opts.ClientId = builder.Configuration["Google:ClientId"]!;
    opts.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
    Console.WriteLine(opts);
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
{
    opts.TokenValidationParameters.ValidateAudience = false;
    opts.TokenValidationParameters.ValidateIssuer = false;
    opts.TokenValidationParameters.IssuerSigningKey
        = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["BearerTokens:Key"]!));
});
builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Identity/SignIn";
    opts.LogoutPath = "/Identity/SignOut";
    opts.AccessDeniedPath = "/Identity/Forbidden";
    opts.Events.DisableRedirectionForApiClients();
});

builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5100")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();
app.UseRouting();
app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapRazorPages();

app.SeedUserStoreForDashboard();
app.Run();
