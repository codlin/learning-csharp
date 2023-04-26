/// <summary>
/// 使用 cookie 来存储应用程序的状态数据，为中间件组件提供所需的数据。这种方法的问题是 cookie 的内容存储在客户端，
/// 在那里它可以被操纵并用于改变应用程序的行为。
/// 更好的方法是使用 ASP.NET Core 会话功能。会话中间件向响应添加一个 cookie，它允许识别相关请求，并且还与存储在服务器上的数据相关联。
/// 当收到包含会话 cookie 的请求时，会话中间件组件会检索与会话关联的服务器端数据，并通过 HttpContext 对象将其提供给其他中间件组件。
/// 使用会话意味着应用程序的数据保留在服务器上，只有会话的标识符被发送到浏览器。
/// </summary>

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opts => {
    opts.IdleTimeout = TimeSpan.FromMinutes(30);
    // 此属性指定应用程序运行是否需要 cookie，即使用户已指定他们不希望应用程序使用 cookie，也应使用 cookie。默认值为假。
    // 默认情况下，会话 cookie 未表示为必需的，这在使用 cookie 同意时可能会导致问题。
    // 将 IsEssential 属性设置为 true 以确保会话始终有效。如果您发现会话未按预期工作，
    // 那么这可能是原因，您必须将 IsEssential 设置为 true 或调整您的应用程序以处理不同意且不接受会话 cookie 的用户。
    opts.Cookie.IsEssential = true;
});

var app = builder.Build();
app.UseSession();
app.UseMiddleware<Platform.ConsentMiddleware>();

app.MapFallback(async context => await context.Response.WriteAsync("Hello World!"));

app.Run();