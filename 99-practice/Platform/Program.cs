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

((IApplicationBuilder)app).Map("/branch", branch => {
    // // branch.Use(async (HttpContext context, Func<Task> next) => {
    // branch.Run(async (context) => {
    //     await context.Response.WriteAsync($"Branch Middleware");
    // });
    branch.Run(new Platform.QueryStringMiddleWare().Invoke);
});

app.UseMiddleware<Platform.QueryStringMiddleWare>();

app.MapGet("/", () => "Hello World!");

app.Run();
