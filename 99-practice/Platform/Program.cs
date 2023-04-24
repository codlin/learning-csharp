using Microsoft.Extensions.Options;
using Platform;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<MessageOptions>(options => {
    options.CityName = "Albany";
});

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
    branch.Run(new QueryStringMiddleWare().Invoke);
});

app.UseMiddleware<QueryStringMiddleWare>();

app.MapGet("/", () => "Hello World!");
app.MapGet("/location", async (HttpContext context, IOptions<MessageOptions> msgOpts) => {
    Platform.MessageOptions opts = msgOpts.Value;
    await context.Response.WriteAsync($"{opts.CityName}, {opts.CountryName}");
});

app.Run();
