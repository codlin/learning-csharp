using Platform;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<RouteOptions>(opts => {
    opts.ConstraintMap.Add("countryName", typeof(CountryRouteConstraint));
});

var app = builder.Build();
app.Use(async (context, next) => {
    Endpoint? end = context.GetEndpoint();
    if (end != null) {
        await context.Response.WriteAsync($"{end.DisplayName} Selected \n");
    } else {
        await context.Response.WriteAsync("No Endpoint Selected \n");
    }
    await next();
});
app.Map("{number:int}", async context => {
    await context.Response.WriteAsync("Routed to the int endpoint");
}).WithDisplayName("Int Endpoint")
.Add(b => ((RouteEndpointBuilder)b).Order = 1);

app.Map("{number:double}", async context => {
    await context.Response
    .WriteAsync("Routed to the double endpoint");
}).WithDisplayName("Double Endpoint")
.Add(b => ((RouteEndpointBuilder)b).Order = 2);

app.MapGet("capital/{country:countryName}", Capital.Endpoint);
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
}).WithDisplayName("Not found Endpoint");

app.Run();
