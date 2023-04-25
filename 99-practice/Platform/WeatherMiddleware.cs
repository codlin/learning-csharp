using Platform.Services;
namespace Platform;

public class WeatherMiddleware {
    private RequestDelegate next;
    // private IResponseFormatter formatter;
    public WeatherMiddleware(RequestDelegate nextDelegate) {
        next = nextDelegate;
        // formatter = respFormatter;
    }
    public async Task Invoke(HttpContext context, IResponseFormatter formatter) {
        await formatter.Format(context, "Middleware Class: It will be raining in London\n");
        if (context.Request.Path == "/middleware/class") {
            await formatter.Format(context, "Middleware Class: It is raining in London");
        } else {
            await next(context);
        }
    }
}