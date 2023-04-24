var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Use(async (context, next) => {
    await next();
    await context.Response.WriteAsync($"\nStatus Code: {context.Response.StatusCode}");
});

app.Use(async (context, next) => {
    if (context.Request.Path == "/short") {
        await context.Response.WriteAsync($"Request Short Circuited");
    } else {
        await next();
    }
});

app.UseMiddleware<Platform.QueryStringMiddleware>();

app.MapGet("/", () => "Hello World!");

app.Run();
