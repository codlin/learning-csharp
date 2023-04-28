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

    /// 新的操作方法用 Consumes 属性修饰，限制了每个方法可以处理的数据类型。
    /// 属性的组合意味着 Content-Type 标头为 application/json 的 HTTP POST 属性将由 SaveProductJson 操作方法处理。 
    /// Content-Type 标头为 application/xml 的 HTTP POST 请求将由 SaveProductXml 操作方法处理。
    [HttpPost]
    [Consumes("application/json")]
    public string SaveProductJson(ProductBindingTarget product)
    {
        return $"JSON: {product.Name}";
    }

    /*
    当您同时负责开发 Web 服务及其客户端时，每个操作的目的及其结果都是显而易见的，并且通常同时编写。
    如果您负责第三方开发人员使用的 Web 服务，那么您可能需要提供描述该 Web 服务如何工作的文档。 
    OpenAPI 规范（也称为 Swagger）以其他程序员可以理解并以编程方式使用的方式描述 Web 服务。
    在本节中，我将演示如何使用 OpenAPI 来描述 Web 服务，并向您展示如何微调该描述。 
    OpenAPI 发现过程需要每个操作方法的 HTTP 方法和 URL 模式的唯一组合。
    该流程不支持 Consumes 属性，因此需要更改 ContentController 以删除接收 XML 和 JSON 数据的单独操作，如清单 20-25 所示。
    */
    // [HttpPost]
    // [Consumes("application/xml")]
    // public string SaveProductXml(ProductBindingTarget product)
    // {
    //     return $"XML: {product.Name}";
    // }
}
