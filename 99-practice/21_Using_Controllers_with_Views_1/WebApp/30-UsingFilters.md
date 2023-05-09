# Using Filters

过滤器将额外的逻辑注入到请求处理中。过滤器就像应用于单个端点的中间件，可以是操作或页面处理程序方法，它们提供了一种优雅的方式来管理一组特定的请求。在本章中，我将解释过滤器的工作原理，描述 ASP.NET Core 支持的不同类型的过滤器，并演示自定义过滤器的使用和 ASP.NET Core 提供的过滤器。表 30-1 对本章进行了总结。

过滤器允许将原本应用于中间件组件或操作方法的逻辑定义在可以轻松重用的类中。假设您想对某些操作方法强制执行 HTTPS 请求。在第 16 章中，我通过阅读 HttpRequest 对象的 IsHttps 属性向您展示了如何在中间件中完成此操作。这种方法的问题是中间件必须了解路由系统的配置才能知道如何拦截对特定操作方法的请求。一种更有针对性的方法是读取操作方法中的 HttpRequest.IsHttps 属性，如清单 30-10 所示。  
Listing 30-10. Selectively Enforcing HTTPS in the HomeController.cs File in the Controllers Folder
```cs
..
public IActionResult Index() {
  if (Request.IsHttps) {} }
...
```
这种方法有效但存在问题。第一个问题是 action 方法包含的代码更多地是关于实施安全策略而不是处理请求。一个更严重的问题是，在 action 方法中包含 HTTP 检测代码不能很好地扩展，并且必须在控制器中的每个 action 方法中复制，如清单 30-11 所示。  
Listing 30-11. Adding Action Methods in the HomeController.cs File in the Controllers Folder
```cs
public IActionResult Secure()
{
    if (Request.IsHttps)
    {
        return View("Message",
        "This is the Secure action on the Home controller");
    }
    else
    {
        return new StatusCodeResult(StatusCodes.Status403Forbidden);
    }
}
```
我必须记住在我想要要求 HTTPS 的每个控制器中的每个操作方法中实施相同的检查。实现安全策略的代码是控制器的重要组成部分——公认的简单——这使得控制器更难理解，我忘记将它添加到新的操作方法中只是时间问题，造成一个漏洞在我的安全策略中。这是过滤器解决的问题类型。清单 30-12 替换了我对 HTTPS 的检查并实现了一个过滤器。  
Listing 30-12. Applying a Filter in the HomeController.cs File in the Controllers Folder
```cs
[RequireHttps]
public IActionResult Index()
{
    return View("Message",
    "This is the Index action on the Home controller");
}
[RequireHttps]
public IActionResult Secure()
{
    return View("Message",
    "This is the Secure action on the Home controller");
}
```
RequireHttps 属性应用 ASP.NET Core 提供的内置过滤器之一。此过滤器限制对操作方法的访问，以便仅支持 HTTPS 请求，并允许我从每个方法中删除安全代码并专注于处理成功的请求。
我必须仍然记得将 RequireHttps 属性应用于每个操作方法，这意味着我可能会忘记。但是过滤器有一个有用的技巧：**将特性应用于控制器类与将它应用于每个单独的操作方法具有相同的效果**，如清单 30-13 所示。
```cs
[RequireHttps]
public class HomeController : Controller
```
可以应用不同粒度级别的过滤器。如果您想限制对某些操作的访问而不是其他操作，则可以将 RequireHttps 属性仅应用于这些方法。如果您想保护所有操作方法，包括您将来添加到控制器的任何方法，则可以将 RequireHttps 属性应用于该类。如果你想对应用程序中的每个动作应用一个过滤器，那么你可以使用全局过滤器，我将在本章后面介绍。

## Using Filters in Razor Pages
筛选器也可用于 Razor Pages。例如，要在 Message Razor 页面中实施仅 HTTPS 策略，我必须添加一个检查连接的处理程序方法，如清单 30-14 所示。  
Listing 30-14. Checking Connections in the Message.cshtml File in the Pages Folder
```cs
public class MessageModel : PageModel {
    ...
    public IActionResult OnGet()
    {
        if (!Request.IsHttps)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
        else
        {
            return Page();
        }
    }
}
```
处理程序方法有效，但它很笨拙，并且会出现与操作方法遇到的相同问题。在 Razor Pages 中使用过滤器时，该属性可以应用于处理程序方法，或者如清单 30-15 所示应用于整个类。  
Listing 30-15. Applying a Filter in the Message.cshtml File in the Pages Folder
```cs
[RequireHttps]
public class MessageModel : PageModel
```

# Understanding Filters
ASP.NET Core 支持不同类型的筛选器，每种筛选器都有不同的用途。表 30-2 描述了过滤器类别。  
Table 30-2. The Filter Types  
| Name | Description |
| Authorization filters | 这种类型的过滤器用于应用程序的授权策略。|
| Resource filters | 此类过滤器用于拦截请求，通常用于实现缓存等功能。|
| Action filters | 这种类型的过滤器用于在操作方法`action method`接收到请求之前修改请求，或者在操作结果生成之后修改操作结果。这种类型的过滤器只能应用于控制器和操作。|
| Page filters | 这种类型的筛选器用于在 Razor 页面处理程序方法接收请求之前修改请求，或者在生成操作结果之后修改操作结果。这种类型的筛选器只能应用于 Razor Pages。|
| Result filters | 这种类型的过滤器用于在执行前更改操作结果或在执行后修改结果。|
| Exception filters | 这种类型的过滤器用于处理在执行操作方法或页面处理程序期间发生的异常。|

过滤器有自己的管道，并按特定顺序执行，如图 30-5 所示。
```
            +-----------------------------------------------------------------------------------------------+
            | Filter Pipeline                                                                               |
            |   +---------------+    +------------+           +- - - - - - - +            +------------+    |   +------------+
Request ----+-->| Authorization |--->|            |---------> |   Model      |----------->|            |----+-->|            |
            |   | Files         |    |  Resource  |           |   Binding    |            |  Resource  |    |   |            |
            |   +---------------+    |  Filters   |           +- - - - - - - +            |  Filters   |    |   |  Endpoint  |
            |                        |            |   +------------+   +------------+     |            |    |   |            |
Response <--+------------------------|            |<--|   Result   |<--| Exception  |<----|            |<---+---|            |
            |                        |            |   |   Filter   |   |  Filters   |     |            |    |   |            |
            |                        +------------+   +------------+   +------------+     +------------+    |   +------------+ 
            +-----------------------------------------------------------------------------------------------+ 
```
过滤器可以使过滤器管道**短路**，以防止将请求转发到下一个过滤器。例如，如果用户未经身份验证，授权过滤器可以使管道短路并返回错误响应。资源、操作`action`和页面`page`过滤器能够在端点处理请求之前和之后检查请求，从而允许这些类型的过滤器使管道短路；在处理请求之前更改请求；或改变响应。 
（我在图 30-5 中简化了过滤器的流程。**页面过滤器在模型绑定过程之前和之后运行**，如“了解页面过滤器”部分所述。）    
每种类型的过滤器都是使用 ASP.NET 定义的接口实现的核心，它还提供基类，可以轻松地将某些类型的过滤器作为属性应用。我在接下来的部分中描述了每个接口和属性类，但它们显示在表 30-3 中以供快速参考。  
Table 30-3. The Filter Types, Interfaces, and Attribute Base Classes  
| Filter Type | Interfaces | Attribute Class |
|-|-|-|
| Authorization filters | IAuthorizationFilter IAsyncAuthorizationFilter | No attribute class is provided. |
| Resource filters | IResourceFilter IAsyncResourceFilter | No attribute class is provided. |
| Action filters | IActionFilterIAsyncActionFilter | ActionFilterAttribute |
| Page filters | IPageFilterIAsyncPageFilter | No attribute class is provided. |
| Result filters | IResultFilterIAsyncResultFilterIAlwaysRunResult FilterIAsyncAlwaysRunResultFilter | ResultFilterAttribute |
| Exception Filters | IExceptionFilterIAsyncExceptionFilter | ExceptionFilterAttribute |

# Creating Custom Filters
筛选器实现 IFilterMetadata 接口，该接口位于 Microsoft.AspNetCore.Mvc.Filters 命名空间中。这是接口：
```cs
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IFilterMetadata { }
}
```
该接口是空的，不需要过滤器来实现任何特定的行为。这是因为上一节中描述的每个过滤器类别都以不同的方式工作。过滤器以 FilterContext 对象的形式提供上下文数据。为方便起见，表 30-4 描述了 FilterContext 提供的属性。  
Table 30-4. The FilterContext Properties  
| Name | Description |
|-|-|
| ActionDescriptor | 此属性返回一个 ActionDescriptor 对象，它描述了操作方法。 |
| HttpContext | 此属性返回一个 HttpContext 对象，它提供 HTTP 请求的详细信息和将作为返回发送的 HTTP 响应。 |
| ModelState | 该属性返回一个 ModelStateDictionary 对象，用于验证客户端发送的数据。 |
| RouteData | 此属性返回一个 RouteData 对象，该对象描述路由系统处理请求的方式。 |
| Filters | 此属性返回已应用于操作方法的过滤器列表，表示为 IList\<IFilterMetadata\>。 |

