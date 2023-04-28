using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
namespace WebApp.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ContentController : ControllerBase
{
    private DataContext context;
    public ContentController(DataContext dataContext)
    {
        context = dataContext;
    }
    [HttpGet("string")]
    public string GetString() => "This is a string response";

    /// <summary>
    /// Accept 标头并不总是在编写客户端的程序员的控制之下。
    /// 在这种情况下，允许使用 URL 请求响应的数据格式会很有帮助。
    /// 通过使用 FormatFilter 属性装饰 action 方法并确保在 action 方法的路由中有一个格式段变量来启用此功能。
    /// 该过滤器从匹配请求的路由中获取格式段变量的值，并使用它来覆盖接受客户端发送的标头。
    /// 我还扩展了 Produces 属性指定的类型范围，以便操作方法可以返回 JSON 和 XML 响应。
    /// 应用程序支持的每种数据格式都有一个简写形式：xml 表示 XML 数据，json 表示 JSON 数据。
    /// 当包含这些简写名称之一的 URL 以操作方法为目标时，将忽略 Accept 标头，并使用指定的格式。
    /// </summary>
    /// <returns></returns>
    [HttpGet("object/{format?}")]
    [FormatFilter]
    [Produces("application/json", "application/xml")]
    public async Task<ProductBindingTarget> GetObject()
    {
        Product p = await context.Products.FirstAsync();
        return new ProductBindingTarget()
        {
            Name = p.Name,
            Price = p.Price,
            CategoryId = p.CategoryId,
            SupplierId = p.SupplierId
        };
    }
}