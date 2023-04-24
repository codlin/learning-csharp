using Platform;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// app.MapGet("files/{filename}.{ext}", async context => {
// app.MapGet("{first}/{second}/{*catchall}", async context => {
// app.MapGet("{first:int}/{second:bool}", async context => {
app.MapGet("{first:alpha:length(3)}/{second:bool}", async context => {
    await context.Response.WriteAsync("Request Was Routed\n");
    foreach (var kvp in context.Request.RouteValues) {
        await context.Response
        .WriteAsync($"{kvp.Key}: {kvp.Value}\n");
    }
});
// app.MapGet("capital/{country=France}", Capital.Endpoint);
app.MapGet("capital/{country:regex(^uk|france|monaco$)}", Capital.Endpoint);
app.MapGet("size/{city?}", Population.Endpoint).WithMetadata(new RouteNameMetadata("population"));

app.MapFallback(async context => {
    await context.Response.WriteAsync("Routed to fallback endpoint");
});

app.Run();
