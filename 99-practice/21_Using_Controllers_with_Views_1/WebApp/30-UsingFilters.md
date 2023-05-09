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
