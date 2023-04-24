using Platform;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// app.MapGet("files/{filename}.{ext}", async context => {
app.MapGet("{first}/{second}/{*catchall}", async context => {
    await context.Response.WriteAsync("Request Was Routed\n");
    foreach (var kvp in context.Request.RouteValues) {
        await context.Response
        .WriteAsync($"{kvp.Key}: {kvp.Value}\n");
    }
});
app.MapGet("capital/{country=France}", Capital.Endpoint);
app.MapGet("size/{city?}", Population.Endpoint).WithMetadata(new RouteNameMetadata("population"));

app.Run();
