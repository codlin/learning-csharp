var builder = WebApplication.CreateBuilder(args);

// enable MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// enable MVC
app.MapDefaultControllerRoute();

app.Run();
