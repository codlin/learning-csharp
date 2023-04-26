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

builder.Services.AddHttpsRedirection(opts => {
    opts.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    opts.HttpsPort = 5500;
});

var app = builder.Build();
app.UseHttpsRedirection();


app.UseSession();
app.UseMiddleware<Platform.ConsentMiddleware>();

app.MapGet("/session", async context => {
    int counter1 = (context.Session.GetInt32("counter1") ?? 0) + 1;
    int counter2 = (context.Session.GetInt32("counter2") ?? 0) + 1;
    context.Session.SetInt32("counter1", counter1);
    context.Session.SetInt32("counter2", counter2);
    // 不必处理会话 cookie、检测过期会话或从缓存中加载会话数据。
    // 所有这些工作都由会话中间件自动完成，它通过 HttpContext.Session 属性显示结果。
    // 这种方法的一个结果是 HttpContext.Session 属性直到会话中间件处理完请求后才填充数据，
    // 这意味着您应该尝试仅在中间件或在之后添加到请求管道的端点中访问会话数据UseSession 方法被调用。
    await context.Session.CommitAsync();
    await context.Response.WriteAsync($"Counter1: {counter1}, Counter2: {counter2}");
});

app.MapFallback(async context => {
    await context.Response.WriteAsync($"HTTPS Request: {context.Request.IsHttps} \n");
    await context.Response.WriteAsync("Hello World!");
});

app.Run();