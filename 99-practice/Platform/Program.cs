using Platform;
using Platform.Services;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddSingleton<IResponseFormatter, HtmlResponseFormatter>();
// builder.Services.AddTransient<IResponseFormatter, GuidService>();
// builder.Services.AddScoped<IResponseFormatter, GuidService>();
// builder.Services.AddScoped<IResponseFormatter, TimeResponseFormatter>();
// builder.Services.AddScoped<ITimeStamper, DefaultTimeStamper>();
// IWebHostEnvironment env = builder.Environment;
// if (env.IsDevelopment()) {
//     builder.Services.AddScoped<IResponseFormatter, TimeResponseFormatter>();
//     builder.Services.AddScoped<ITimeStamper, DefaultTimeStamper>();
// } else {
//     builder.Services.AddScoped<IResponseFormatter, HtmlResponseFormatter>();
// }
IConfiguration config = builder.Configuration;
builder.Services.AddScoped<IResponseFormatter>(serviceProvider => {
    string? typeName = config["services:IResponseFormatter"];
    return (IResponseFormatter)ActivatorUtilities
    .CreateInstance(serviceProvider, typeName == null
    ? typeof(GuidService) : Type.GetType(typeName, true)!);
});
builder.Services.AddScoped<ITimeStamper, DefaultTimeStamper>();

var app = builder.Build();
app.UseMiddleware<WeatherMiddleware>();

app.MapGet("middleware/function", async (HttpContext context, IResponseFormatter formatter) => {
    await formatter.Format(context, "Middleware Function: It is snowing in Chicago");
});

// app.MapGet("endpoint/class", WeatherEndpoint.Endpoint);
// app.MapWeather("endpoint/class");
app.MapEndpoint<WeatherEndpoint>("endpoint/class");

app.MapGet("endpoint/function", async (HttpContext context) => {
    IResponseFormatter formatter = context.RequestServices.GetRequiredService<IResponseFormatter>();
    await formatter.Format(context, "Endpoint Function: It is sunny in LA");
});

app.Run();
