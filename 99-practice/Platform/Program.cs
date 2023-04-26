using Microsoft.AspNetCore.HttpLogging;
using Platform;

var builder = WebApplication.CreateBuilder(args);
var servicesConfig = builder.Configuration;
// - use configuration settings to set up services
builder.Services.Configure<MessageOptions>(servicesConfig.GetSection("Location"));
builder.Services.AddHttpLogging(opts => {
    opts.LoggingFields = HttpLoggingFields.RequestMethod
    | HttpLoggingFields.RequestPath | HttpLoggingFields.ResponseStatusCode;
});

var app = builder.Build();
app.UseHttpLogging();

app.UseMiddleware<LocationMiddleware>();

app.MapGet("config", async (HttpContext context, IConfiguration config, IWebHostEnvironment env) => {
    string defaultDebug = config["Logging:LogLevel:Default"];
    await context.Response.WriteAsync($"The config setting is: {defaultDebug}");
    await context.Response.WriteAsync($"\nThe env setting is: {env.EnvironmentName}");
    string wsID = config["WebService:Id"];
    string wsKey = config["WebService:Key"];
    await context.Response.WriteAsync($"\nThe secret ID is: {wsID}");
    await context.Response.WriteAsync($"\nThe secret Key is: {wsKey}");
});

app.MapGet("/", async context => {
    await context.Response.WriteAsync("Hello World!");
});

app.MapGet("population/{city?}", Population.Endpoint);

app.Logger.LogDebug("Pipeline configuration complete");

app.Run();
