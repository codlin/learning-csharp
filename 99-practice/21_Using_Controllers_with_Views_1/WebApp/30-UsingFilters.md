# Using Filters

过滤器将额外的逻辑注入到请求处理中。过滤器就像应用于单个端点的中间件，可以是 action 或页面处理程序方法，它们提供了一种优雅的方式来管理一组特定的请求。在本章中，我将解释过滤器的工作原理，描述 ASP.NET Core 支持的不同类型的过滤器，并演示自定义过滤器的使用和 ASP.NET Core 提供的过滤器。表 30-1 对本章进行了总结。

过滤器允许将原本应用于中间件组件或`action`方法的逻辑定义在可以轻松重用的类中。假设您想对某些`action`方法强制执行 HTTPS 请求。在第 16 章中，我通过阅读 HttpRequest 对象的 IsHttps 属性向您展示了如何在中间件中完成此 action 。这种方法的问题是中间件必须了解路由系统的配置才能知道如何拦截对特定`action`方法的请求。一种更有针对性的方法是读取`action`方法中的 HttpRequest.IsHttps 属性，如清单 30-10 所示。  
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
我必须记住在我想要要求 HTTPS 的每个控制器中的每个`action`方法中实施相同的检查。实现安全策略的代码是控制器的重要组成部分——公认的简单——这使得控制器更难理解，我忘记将它添加到新的`action`方法中只是时间问题，造成一个漏洞在我的安全策略中。这是过滤器解决的问题类型。清单 30-12 替换了我对 HTTPS 的检查并实现了一个过滤器。  
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
RequireHttps 属性应用 ASP.NET Core 提供的内置过滤器之一。此过滤器限制对`action`方法的访问，以便仅支持 HTTPS 请求，并允许我从每个方法中删除安全代码并专注于处理成功的请求。
我必须仍然记得将 RequireHttps 属性应用于每个`action`方法，这意味着我可能会忘记。但是过滤器有一个有用的技巧：**将特性应用于控制器类与将它应用于每个单独的`action`方法具有相同的效果**，如清单 30-13 所示。
```cs
[RequireHttps]
public class HomeController : Controller
```
可以应用不同粒度级别的过滤器。如果您想限制对某些 action 的访问而不是其他 action ，则可以将 RequireHttps 属性仅应用于这些方法。如果您想保护所有`action`方法，包括您将来添加到控制器的任何方法，则可以将 RequireHttps 属性应用于该类。如果你想对应用程序中的每个动作应用一个过滤器，那么你可以使用全局过滤器，我将在本章后面介绍。

## Using Filters in Razor Pages
过滤器也可用于 Razor Pages。例如，要在 Message Razor 页面中实施仅 HTTPS 策略，我必须添加一个检查连接的处理程序方法，如清单 30-14 所示。  
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
处理程序方法有效，但它很笨拙，并且会出现与`action`方法遇到的相同问题。在 Razor Pages 中使用过滤器时，该属性可以应用于处理程序方法，或者如清单 30-15 所示应用于整个类。  
Listing 30-15. Applying a Filter in the Message.cshtml File in the Pages Folder
```cs
[RequireHttps]
public class MessageModel : PageModel
```

# Understanding Filters
ASP.NET Core 支持不同类型的过滤器，每种过滤器都有不同的用途。表 30-2 描述了过滤器类别。  
Table 30-2. The Filter Types  
| Name | Description |
| Authorization filters | 这种类型的过滤器用于应用程序的授权策略。|
| Resource filters | 此类过滤器用于拦截请求，通常用于实现缓存等功能。|
| Action filters | 这种类型的过滤器用于在`action`方法`action method`接收到请求之前修改请求，或者在 action 结果生成之后修改 action 结果。这种类型的过滤器只能应用于控制器和 action 。|
| Page filters | 这种类型的过滤器用于在 Razor 页面处理程序方法接收请求之前修改请求，或者在生成 action 结果之后修改 action 结果。这种类型的过滤器只能应用于 Razor Pages。|
| Result filters | 这种类型的过滤器用于在执行前更改 action 结果或在执行后修改结果。|
| Exception filters | 这种类型的过滤器用于处理在执行`action`方法或页面处理程序期间发生的异常。|

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
过滤器可以使过滤器管道**短路**，以防止将请求转发到下一个过滤器。例如，如果用户未经身份验证，授权过滤器可以使管道短路并返回错误响应。资源、 action `action`和页面`page`过滤器能够在端点处理请求之前和之后检查请求，从而允许这些类型的过滤器使管道短路；在处理请求之前更改请求；或改变响应。 
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
过滤器实现 IFilterMetadata 接口，该接口位于 Microsoft.AspNetCore.Mvc.Filters 命名空间中。这是接口：
```cs
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IFilterMetadata { }
}
```
该接口是空的，不需要过滤器来实现任何特定的行为。这是因为上一节中描述的每个过滤器类别都以不同的方式工作。过滤器以 FilterContext 对象的形式提供上下文数据。为方便起见，表 30-4 描述了 FilterContext 提供的属性。  
Table 30-4. The FilterContext Properties  
| Name | Description |
|-|-|
| ActionDescriptor | 此属性返回一个 ActionDescriptor 对象，它描述了`action`方法。 |
| HttpContext | 此属性返回一个 HttpContext 对象，它提供 HTTP 请求的详细信息和将作为返回发送的 HTTP 响应。 |
| ModelState | 该属性返回一个 ModelStateDictionary 对象，用于验证客户端发送的数据。 |
| RouteData | 此属性返回一个 RouteData 对象，该对象描述路由系统处理请求的方式。 |
| Filters | 此属性返回已应用于`action`方法的过滤器列表，表示为 IList\<IFilterMetadata\>。 |


## Understanding Authorization Filters
授权过滤器用于实施应用程序的安全策略。授权过滤器在其他类型的过滤器之前和端点处理请求之前执行。下面是 IAuthorizationFilter 接口的定义：
```cs
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IAuthorizationFilter : IFilterMetadata {
        void OnAuthorization(AuthorizationFilterContext context);
    }
}
```
调用 OnAuthorization 方法为过滤器提供授权请求的机会。对于异步授权过滤器，这里是 IAsyncAuthorizationFilter 接口的定义：
```cs
using System.Threading.Tasks;
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IAsyncAuthorizationFilter : IFilterMetadata {
        Task OnAuthorizationAsync(AuthorizationFilterContext context);
    }
}
```
调用 OnAuthorizationAsync 方法，以便过滤器可以授权请求。无论使用哪个接口，过滤器都会通过 AuthorizationFilterContext 对象接收描述请求的上下文数据，该对象派生自 FilterContext 类并添加了一个重要属性，如表 30-5 中所述。  
Table 30-5. The AuthorizationFilterContext Property  
| Name | Description |
|-|-|
| Result | 当请求不符合应用程序的授权策略时，授权过滤器会设置此 IActionResult 属性。如果设置了此属性，则 ASP.NET Core 将执行 IActionResult 而不是调用端点。|

### Creating an Authorization Filter
为了演示授权过滤器的工作原理，我在 WebApp 文件夹中创建了一个 Filters 文件夹，添加了一个名为 HttpsOnlyAttribute.cs 的类文件。
如果请求符合授权策略，则授权过滤器不执行任何 action ，并且无 action 允许 ASP.NET Core 继续执行下一个过滤器，并最终执行端点。如果出现问题，过滤器将设置传递给 OnAuthorization 方法的 AuthorizationFilterContext 对象的 Result 属性。这可以防止进一步执行并提供返回给客户端的结果。在清单中，HttpsOnlyAttribute 类检查 HttpRequest 上下文对象的 IsHttps 属性，并设置 Result 属性以在没有 HTTPS 的情况下发出请求时中断执行。授权过滤器可应用于控制器、`action`方法和 Razor 页面。清单 30-17 将新过滤器应用于 Home 控制器。  
Listing 30-17. Applying a Custom Filter in the HomeController.cs File in the Controllers Folder  
```cs
using WebApp.Filters;
...
[HttpsOnly]
public class HomeController : Controller
```

## Understanding Resource Filters
资源过滤器针对每个请求执行两次：在 ASP.NET Core 模型绑定过程之前以及在处理 action 结果以生成结果之前再次执行。这是 IResourceFilter 接口的定义：
```cs
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IResourceFilter : IFilterMetadata {
        void OnResourceExecuting(ResourceExecutingContext context);
        void OnResourceExecuted(ResourceExecutedContext context);
    }
}
```
OnResourceExecuting 方法在处理请求时调用，而 OnResourceExecuted 方法在端点处理请求之后但在执行 action 结果之前调用。对于异步资源过滤器，这里是 IAsyncResourceFilter 接口的定义：  
```cs
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IAsyncResourceFilter : IFilterMetadata {
        Task OnResourceExecutionAsync(ResourceExecutingContext context,
                                      ResourceExecutionDelegate next);
    }
}
```
该接口定义了一个方法，该方法接收上下文对象和要调用的委托。资源过滤器能够在调用委托之前检查请求并在执行之前检查响应。使用 ResourceExecutingContext 类为 OnResourceExecuting 方法提供上下文，除了 FilterContext 类定义的属性外，该类还定义了表 30-6 中所示的属性。  
Table 30-6. The Properties Defined by the ResourceExecutingContext Class  
| Name | Description |
|-|-|
| Result | 此 IActionResult 属性用于提供使管道短路的结果。|
| ValueProviderFactories | 此属性返回一个 IList\<IValueProviderFactory\>，它提供对为模型绑定过程提供值的对象的访问。|

使用 ResourceExecutedContext 类为 OnResourceExecuted 方法提供上下文，除了 FilterContext 类定义的属性外，该类还定义了表 30-7 中所示的属性。  
Table 30-7. The Properties Defined by the ResourceExecutedContext Class  
| Name | Description |
|-|-|
| Result | 此 IActionResult 属性提供将用于生成响应的 action 结果。 |
| Canceled | 如果另一个过滤器通过将 action 结果分配给 ActionExecutingContext 对象的 Result 属性而使管道短路，则此 bool 属性设置为 true。 |
| Exception | 该属性用于存储执行过程中抛出的异常。 |
| ExceptionDispatchInfo | 此方法返回一个 ExceptionDispatchInfo 对象，该对象包含执行期间抛出的任何异常的堆栈跟踪详细信息。 |
| ExceptionHandled | 将此属性设置为 true 表示过滤器已处理异常，该异常将不会进一步传播。 |

### Creating a Resource Filter
资源过滤器通常用于可以使管道短路并尽早提供响应的地方，例如在实现数据缓存时。要创建一个简单的缓存过滤器，请使用清单 30-18 中所示的代码将一个名为 SimpleCacheAttribute.cs 的类文件添加到 Filters 文件夹中。

**FILTERS AND DEPENDENCY INJECTION**
作为属性应用的过滤器不能在其构造函数中声明依赖关系，除非它们实现 IFilterFactory 接口并负责直接创建实例，如本章后面的“创建过滤器工厂”部分所述。

这个过滤器不是特别有用的缓存，但它确实展示了资源过滤器的工作原理。 OnResourceExecuting 方法通过将上下文对象的 Result 属性设置为先前缓存的 action 结果，为过滤器提供了短路管道的机会。如果为 Result 属性分配了一个值，则过滤器管道会短路，并执行 action 结果为客户端生成响应。缓存的 action 结果仅使用一次，然后从缓存中丢弃。如果没有为 Result 属性分配值，则请求将传递到管道中的下一步，这可能是另一个过滤器或端点。    
OnResourceExecuted 方法为过滤器提供管道未短路时产生的 action 结果。在这种情况下，过滤器会缓存 action 结果，以便它可以用于后续请求。资源过滤器可应用于控制器、`action`方法和 Razor 页面。清单 30-19 将自定义资源过滤器应用于 Message Razor 页面并添加了一个时间戳，这将有助于确定何时缓存 action 结果。  
Listing 30-19. Applying a Resource Filter in the Message.cshtml File in the Pages Folder    
```cs
@using WebApp.Filters
[SimpleCache]
public class MessageModel : PageModel
{
    public object Message { get; set; } =
    $"{DateTime.Now.ToLongTimeString()}: This is the Message Razor Page";
}
```
要查看资源过滤器的效果，请重新启动 ASP.NET Core 并请求 https://localhost:44350/pages/message。由于这是路径的第一次请求，因此不会有缓存结果，请求将沿着管道转发。在处理响应时，资源过滤器将缓存 action 结果以供将来使用。重新加载浏览器重复请求，你会看到相同的时间戳，说明已经使用了缓存的动作结果。缓存项在使用时会被删除，这意味着重新加载浏览器将生成带有新时间戳的响应，如图 30-7 所示。

### Creating an Asynchronous Resource Filter
异步资源过滤器的接口使用单一方法接收委托，该委托用于沿过滤器管道转发请求。清单 30-20 重新实现了前面示例中的缓存过滤器，以便它实现 IAsyncResourceFilter 接口。  
Listing 30-20. Creating an Asynchronous Filter in the AsyncSimpleCacheAttribute.cs File in the Filters Folder  

OnResourceExecutionAsync 方法接收一个 ResourceExecutingContext 对象，该对象用于确定管道是否可以短路。如果不能，则在不带参数的情况下调用委托，并在请求已处理并沿着管道返回时异步生成 ResourceExecutedContext 对象。重新启动 ASP.NET Core 并重复上一节中描述的请求，您将看到相同的缓存行为，如图 30-7 所示。  
**警告** 重要的是不要混淆这两个上下文对象。端点产生的 action 结果仅在委托返回的上下文对象中可用。

## Understanding Action Filters
与资源过滤器一样，`action`过滤器执行两次。区别在于 `action` **过滤器在模型绑定过程之后执行，而资源过滤器在模型绑定之前执行**。这意味着资源过滤器可以使管道短路并最大限度地减少 ASP.NET Core 对请求所做的工作。**当需要模型绑定时使用 action 过滤器，这意味着它们用于诸如更改模型或强制验证之类的任务**。`action`过滤器只能应用于控制器和`action`方法，这与资源过滤器不同，资源过滤器也可以与 Razor Pages 一起使用。 （Razor Pages 等同于`action`过滤器是页面过滤器，在“了解页面过滤器”部分中进行了描述。）这是 IActionFilter 接口：  
```cs
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IActionFilter : IFilterMetadata {
        void OnActionExecuting(ActionExecutingContext context);
        void OnActionExecuted(ActionExecutedContext context);
    }
}
```
当`action`过滤器已应用于`action`方法时，OnActionExecuting 方法将在调用 action 方法之前调用，然后在调用 action 方法后调用 OnActionExecuted 方法。通过两个不同的上下文类为`action`过滤器提供上下文数据：OnActionExecuting 方法的 ActionExecutingContext 和 OnActionExecuted 方法的 ActionExecutedContext。    
ActionExecutingContext 类用于描述将要调用的 action ，除了 FilterContext 属性外，还定义了表 30-8 中描述的属性。   
Table 30-8. The ActionExecutingContext Properties  
| Name | Description |
|-|-|
| Controller | 该属性返回将要调用其 action 方法的控制器。 （ action 方法的详细信息可通过从基类继承的 ActionDescriptor 属性获得。）|
| ActionArguments | 此属性返回将传递给 action 方法的参数字典，按名称索引。过滤器可以插入、删除或更改参数。 |
| Result | 如果过滤器将 IActionResult 分配给此属性，则管道将被短路，并且 action 结果将用于生成对客户端的响应，而无需调用 action 方法。|

ActionExecutedContext 类用于表示已执行的 action，除了 FilterContext 属性外，还定义了表 30-9 中描述的属性。  
Table 30-9. The ActionExecutedContext Properties  
| Name | Description |
|-|-|
| Controller | 此属性返回将调用其 action 方法的 Controller 对象。|
| Canceled | 如果另一个 action 过滤器通过将 action 结果分配给 ActionExecutingContext 对象的 Result 属性而使管道短路，则此 bool 属性设置为 true。|
| Exception | 此属性包含 action 方法抛出的任何异常。|
| ExceptionDispatchInfo | 此方法返回一个 ExceptionDispatchInfo 对象，该对象包含 action 方法抛出的任何异常的堆栈跟踪详细信息。|
| ExceptionHandled | 将此属性设置为 true 表示过滤器已处理异常，该异常将不会进一步传播。 |
| Result | 此属性返回由 action 方法生成的 IActionResult。如果需要，过滤器可以更改或替换 action 结果。|

异步`action`过滤器是使用 IAsyncActionFilter 接口实现的。  
```cs
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IAsyncActionFilter : IFilterMetadata {
        Task OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next);
    }
}
```

此接口遵循与本章前面所述的 IAsyncResourceFilter 接口相同的模式。 OnActionExecutionAsync 方法随 ActionExecutingContext 对象和委托一起提供。 ActionExecutingContext 对象在请求被`action`方法接收之前描述请求。过滤器可以通过为 ActionExecutingContext 赋值来使管道短路。结果属性或通过调用委托传递它。委托异步生成一个 ActionExecutedContext 对象，该对象描述`action`方法的结果。

### Creating an Action Filter
将一个名为 ChangeArgAttribute.cs 的类文件添加到 Filters 文件夹，并使用它来定义如清单 30-21 所示的`action`过滤器。  
Listing 30-21. The Contents of the ChangeArgAttribute.cs File in the Filters Folder  

过滤器查找名为 message1 的action参数并更改将用于调用`action`方法的值。将用于`action`方法参数的值由模型绑定过程确定。清单 30-22 向 Home 控制器添加了一个`action`方法并应用了新的过滤器。  
Listing 30-22. Applying a Filter in the HomeController.cs File in the Controllers Folder  

重新启动 ASP.NET Core 并请求 https://localhost:44350/home/messages?message1=hello&message2=world。模型绑定过程将从查询字符串中定位`action`方法定义的参数值。这些值之一随后被`action`过滤器修改，产生如图 30-8 所示的响应。

### Implementing an Action Filter Using the Attribute Base Class
Action 属性也可以通过派生自 ActionFilterAttribute 类来实现，该类扩展了 Attribute 并继承了 IActionFilter 和 IAsyncActionFilter 接口，以便实现类仅覆盖它们需要的方法。在清单 30-23 中，我重新实现了 ChangeArg 过滤器，使其派生自 ActionFilterAttribute。  
Listing 30-23. Using a Filter Base Class in the ChangeArgsAttribute.cs File in the Filters Folder  
```cs
public class ChangeArgAttribute : ActionFilterAttribute  // <--HERE
{
    public override async Task OnActionExecutionAsync( // <-HERE
        ActionExecutingContext context,
        ActionExecutionDelegate next)
}
```

此属性的行为与早期实现的方式相同，基类的使用是偏好问题。重启 ASP.NET Core 并请求 https://localhost:44350/home/messages? message1=hello&message2=world，你会看到如图30-8所示的响应。

### Using the Controller Filter Methods
Controller 类是呈现 Razor 视图的控制器的基础，实现了 IActionFilter 和 IAsyncActionFilter 接口，这意味着您可以定义功能并将其应用于控制器和任何派生控制器定义的 action 。清单 30-24 直接在 HomeController 类中实现了 ChangeArg 过滤器功能。     
Listing 30-24. Using Action Filter Methods in the HomeController.cs File in the Controllers Folder
```cs
using Microsoft.AspNetCore.Mvc.Filters;
...
//[ChangeArg]
...
public override void OnActionExecuting(ActionExecutingContext context) {
    if (context.ActionArguments.ContainsKey("message1")) {
        context.ActionArguments["message1"] = "New message";
    }
}
```
Home 控制器覆盖 OnActionExecuting 方法的 Controller 实现，并使用它来修改将传递给执行方法的参数。重启 ASP.NET Core 并请求 https://localhost:44350/home/messages?message1=hello&message2=world，你将看到如图 30-8 所示的响应。

## Understanding Page Filters
Page 过滤器是等同于`action`过滤器的 Razor Page 过滤器。这是 IPageFilter 接口，由同步页面过滤器实现：  
```cs
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IPageFilter : IFilterMetadata {
        void OnPageHandlerSelected(PageHandlerSelectedContext context);
        void OnPageHandlerExecuting(PageHandlerExecutingContext context);
        void OnPageHandlerExecuted(PageHandlerExecutedContext context);
    }
}
```
OnPageHandlerSelected 方法在 ASP.NET Core 选择页面处理程序方法之后但在执行模型绑定之前调用，这意味着处理程序方法的参数尚未确定。此方法通过 PageHandlerSelectedContext 类接收上下文，除了 FilterContext 类定义的属性外，该类还定义了表 30-10 中所示的属性。此方法不能用于使管道短路，但它可以更改将要接收请求的处理程序方法。   
Table 30-10. The PageHandlerSelectedContext Properties  
| Name | Description |
|-|-|
| ActionDescriptor | 此属性返回 Razor 页面的描述。 |
| HandlerMethod | 此属性返回一个描述所选处理程序方法的 HandlerMethodDescriptor 对象。 |
| HandlerInstance | 此属性返回将处理请求的 Razor 页面实例。 |

OnPageHandlerExecuting 方法在模型绑定过程完成之后但在调用页面处理程序方法之前被调用。此方法通过 PageHandlerExecutingContext 类接收上下文，除了由 PageHandlerSelectedContext 类定义的属性外，该类还定义了表 30-11 中所示的属性。    
Table 30-11. The PageHandlerExecutingContext Properties  
| Name | Description |
|-|-|
| HandlerArguments | 此属性返回包含页面处理程序参数的字典，按名称索引。 |
| Result | 过滤器可以通过将 IActionResult 对象分配给此属性来使管道短路。 |

OnPageHandlerExecuted 方法在调用页面处理程序方法之后但在处理 action 结果以创建响应之前调用。此方法通过 PageHandlerExecutedContext 类接收上下文，除了 PageHandlerExecutingContext 属性外，该类还定义了表 30-12 中所示的属性。    
Table 30-12. The PageHandlerExecutedContext Properties
| Name | Description |
|-|-|
| Canceled | 如果另一个过滤器使过滤器管道短路，则此属性返回 true。 |
| Exception | 如果页面处理程序方法抛出异常，则此属性返回一个异常。 |
| ExceptionHandled | 此属性设置为 true 以指示页面处理程序抛出的异常已由过滤器处理。 |
| Result | 此属性返回将用于为客户端创建响应的 action 结果。 |

异步页面过滤器是通过实现 IAsyncPageFilter 接口创建的，该接口定义如下：    
```cs
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IAsyncPageFilter : IFilterMetadata {
        Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context);
        Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
                PageHandlerExecutionDelegate next);
    }
}
```
OnPageHandlerSelectionAsync 在选择处理程序方法后调用，相当于同步 OnPageHandlerSelected 方法。 OnPageHandlerExecutionAsync 提供了一个 PageHandlerExecutingContext 对象，允许它短路管道和一个被调用以传递请求的委托。委托生成一个 PageHandlerExecutedContext 对象，该对象可用于检查或更改处理程序方法生成的 action 结果。

### Creating a Page Filter
要创建页面过滤器，请将名为 ChangePageArgs.cs 的类文件添加到 Filters 文件夹。  
页面过滤器执行与我在上一节中创建的`action`过滤器相同的任务。在清单 30-26 中，我修改了 Message Razor Page 以定义一个处理程序方法并应用了页面过滤器。页面过滤器可以应用于单独的处理程序方法，或者如清单中那样应用于页面模型类，在这种情况下过滤器用于所有处理程序方法。 （我还在清单 30-26 中禁用了 SimpleCache 过滤器。资源过滤器可以与页面过滤器一起工作。我禁用了这个过滤器是因为缓存响应使一些示例更难理解。）  
Listing 30-26. Using a Page Filter in the Message.cshtml File in the Pages Folder  
```cs
//[SimpleCache]
[ChangePageArgs]
...
public void OnGet(string message1, string message2) {
    Message = $"{message1}, {message2}";
}
```
重新启动 ASP.NET Core 并请求 https://localhost:44350/pages/message?message1=hello&message2=world .页面过滤器将替换 OnGet 处理程序方法的 message1 参数的值，它会产生如图 30-9 所示的响应。

### Using the Page Model Filter Methods
PageModel 类用作页面模型类的基础，实现了 IPageFilter 和 IAsyncPageFilter 接口，这意味着您可以直接向页面模型添加过滤器功能，如清单 30-27 所示。  
Listing 30-27. Using the PageModel Filter Methods in the Message.cshtml File in the Pages Folder  
```cs
@using Microsoft.AspNetCore.Mvc.Filters
...
//[SimpleCache]
//[ChangePageArgs]
...
public override void OnPageHandlerExecuting(
    PageHandlerExecutingContext context)
{
    if (context.HandlerArguments.ContainsKey("message1"))
    {
        context.HandlerArguments["message1"] = "New message";
    }
}
```
请求 https://localhost:44350/pages/message?message1=hello&message2=world。清单 30-27 中页面模型类实现的方法将产生与图 30-9 所示相同的结果。

## Understanding Result Filters
结果过滤器在 action 结果用于生成响应之前和之后执行，允许在端点处理响应之后修改响应。下面是 IResultFilter 接口的定义： 
```cs
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IResultFilter : IFilterMetadata {
        void OnResultExecuting(ResultExecutingContext context);
        void OnResultExecuted(ResultExecutedContext context);
    }
}
```
在端点产生 action 结果后调用 OnResultExecuting 方法。此方法通过 ResultExecutingContext 类接收上下文，除了 FilterContext 类定义的属性外，该类还定义了表 30-13 中描述的属性。   
Table 30-13. The ResultExecutingContext Class Properties
| Name | Description |
|-|-|
| Controller | 此属性返回包含端点的对象。|
| Cancel | 将此属性设置为 true 将使结果过滤器管道短路。|
| Result | 此属性返回端点产生的 action 结果。|

OnResultExecuted 方法在 action 结果被用于生成客户端响应后执行。此方法通过 ResultExecutedContext 类接收上下文，该类定义了表 30-14 中显示的属性，以及它从 FilterContext 类继承的属性。异步结果过滤器实现 IAsyncResultFilter 接口，其定义如下：    
Table 30-14. The ResultExecutedContext Class
| Name | Description |
|-|-|
| Canceled | 如果另一个过滤器使过滤器管道短路，则此属性返回 true。|
| Controller | 此属性返回包含端点的对象。|
| Exception | 如果页面处理程序方法抛出异常，则此属性返回一个异常。|
| ExceptionHandled | 此属性设置为 true 以指示页面处理程序抛出的异常已由过滤器处理。|
| Result | 此属性返回将用于为客户端创建响应的 action 结果。此属性是只读的。|

异步结果过滤器实现了 IAsyncResultFilter 接口，其定义如下：
```cs
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IAsyncResultFilter : IFilterMetadata {
        Task OnResultExecutionAsync(ResultExecutingContext context,
            ResultExecutionDelegate next);
    }
}
```
该接口遵循由其他过滤器类型建立的模式。 OnResultExecutionAsync 方法是通过上下文对象调用的，上下文对象的 Result 属性可用于更改响应和将沿管道转发响应的委托。

### Understanding Always-Run Result Filters
实现 IResultFilter 和 IAsyncResultFilter 接口的过滤器仅在端点正常处理请求时使用。如果另一个过滤器使管道短路或出现异常，则不会使用它们。需要检查或更改响应（即使管道短路）的过滤器可以实现 IAlwaysRunResultFilter 或 IAsyncAlwaysRunResultFilter 接口。这些接口派生自 IResultFilter 和 IAsyncResultFilter 但未定义新功能。相反，ASP.NET Core 会检测始终运行的接口并始终应用过滤器。

### Creating a Result Filter
将名为 ResultDiagnosticsAttribute.cs 的类文件添加到 Filters 文件夹。  
此过滤器检查请求以查看它是否包含名为 diag 的查询字符串参数。如果是，则过滤器会创建一个显示诊断信息的结果，而不是端点生成的输出。清单 30-28 中的过滤器将与 Home 控制器或 Message Razor 页面定义的 action 一起工作。  
请注意，当我在清单 30-28 中创建 action 结果时，我为视图使用了一个完全限定的名称。这避免了应用于 Razor Pages 的过滤器出现问题，其中 ASP.NET Core 尝试将新结果作为 Razor Pages 执行并抛出有关模型类型的异常。  
清单 30-29 将结果过滤器应用于 Home 控制器。
```cs
[ResultDiagnostics]
```
重新启动 ASP.NET Core 并请求 https://localhost:44350/?diag 。查询字符串参数将被过滤器检测到，这将生成如图 30-10 所示的诊断信息。

### Implementing a Result Filter Using the Attribute Base Class
ResultFilterAttribute 类派生自 Attribute 并实现 IResultFilter 和 IAsyncResultFilter 接口，可用作结果过滤器的基类，如清单 30-30 所示。始终运行的接口没有属性基类。  
Listing 30-30. Using the Attribute Base Class in the ResultDiagnosticsAttribute.cs File in the Filters Folder  

重新启动 ASP.NET Core 并请求 https://localhost:44350/?diag。过滤器将产生如图 30-10 所示的输出。

## Understanding Exception Filters
异常过滤器允许您响应异常而无需在每个 action 方法中编写 try...catch 块。异常过滤器可以应用于控制器类、 action 方法、页面模型类或处理程序方法。当端点或已应用于端点的 action 、页面和结果过滤器未处理异常时，将调用它们。 （ action 、页面和结果过滤器可以通过将它们的上下文对象的 ExceptionHandled 属性设置为 true 来处理未处理的异常。）异常过滤器实现 IExceptionFilter 接口，其定义如下：
```cs
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IExceptionFilter : IFilterMetadata {
        void OnException(ExceptionContext context);
    }
}
```
如果遇到未处理的异常，则调用 OnException 方法。 IAsyncExceptionFilter 接口可用于创建异步异常过滤器。下面是异步接口的定义：
```cs
using System.Threading.Tasks;
namespace Microsoft.AspNetCore.Mvc.Filters {
    public interface IAsyncExceptionFilter : IFilterMetadata {
        Task OnExceptionAsync(ExceptionContext context);
}
}
```
OnExceptionAsync 方法是 IExceptionFilter 接口中 OnException 方法的异步对应方法，在出现未处理的异常时调用。对于这两个接口，上下文数据是通过 ExceptionContext 类提供的，该类派生自 FilterContext 并定义了表 30-15 中所示的其他属性。  
Table 30-15. The ExceptionContext Properties  
| Name | Description |
|-|-|
| Exception | 此属性包含抛出的任何异常。|
| ExceptionHandled | 此 bool 属性用于指示异常是否已被处理。|
| Result | 此属性设置将用于生成响应的 IActionResult。|

### Creating an Exception Filter
异常过滤器可以通过实现过滤器接口之一或从 ExceptionFilterAttribute 类派生来创建，该类派生自 Attribute 并实现 IExceptionFilter 和 IAsyncException 过滤器。异常过滤器最常见的用途是为特定异常类型显示自定义错误页面，以便为用户提供比标准错误处理功能所能提供的更有用的信息。  
要创建异常过滤器，请将名为 RangeExceptionAttribute.cs 的类文件添加到 Filters 文件夹。  
此过滤器使用 ExceptionContext 对象获取未处理异常的类型，如果类型为 ArgumentOutOfRangeException ，则创建向用户显示消息的 action 结果。清单 30-32 向我已应用异常过滤器的 Home 控制器添加了一个`action`方法。   
Listing 30-32. Applying an Exception Filter in the HomeController.cs File in the Controllers Folder  
```cs
[RangeException]
public ViewResult GenerateException(int? id)
{
    if (id == null)
    {
        throw new ArgumentNullException(nameof(id));
    }
    else if (id > 10)
    {
        throw new ArgumentOutOfRangeException(nameof(id));
    }
    else
    {
        return View("Message", $"The value is {id}");
    }
}
```
GenerateException `action` 方法依赖默认路由模式从请求 URL 接收可为空的 int 值。如果没有匹配的 URL 段，则 action 方法抛出 ArgumentNullException；如果其值大于 10，则抛出 ArgumentOutOfRangeException 。如果有值且在范围内，则 action 方法返回 ViewResult。  
重新启动 ASP.NET Core 并请求 https://localhost:44350/Home/GenerateException/100 。最终段将超出`action`方法预期的范围，这将抛出过滤器处理的异常类型，产生如图 30-11 所示的结果。如果你请求 /Home/GenerateException，那么过滤器将不会处理 action 方法抛出的异常，将使用默认的错误处理。

# Managing the Filter Lifecycle
默认情况下，ASP.NET Core 管理它创建的过滤器对象并将它们重用于后续请求。这并不总是期望的行为，在接下来的部分中，我将描述不同的方法来控制过滤器的创建方式。要创建一个将显示生命周期的过滤器，请将一个名为 GuidResponseAttribute.cs 的类文件添加到 Filters 文件夹，并使用它来定义清单 30-33 中所示的过滤器。此结果过滤器将终结点生成的 action 结果替换为将呈现消息视图并显示唯一 GUID 值的结果。过滤器被配置为可以多次应用于同一目标，如果管道中较早的过滤器创建了合适的结果，它将添加一条新消息。清单 30-34 将过滤器应用到 Home 控制器两次。 （为简洁起见，我还删除了除其中一个`action`方法之外的所有方法。）要确认过滤器正在重用，请重新启动 ASP.NET Core 并请求 https://localhost:44350/?diag。响应将包含来自两个 GuidResponse 过滤器属性的 GUID 值。已创建过滤器的两个实例来处理请求。重新加载浏览器，您将看到显示相同的 GUID 值，表明为处理第一个请求而创建的过滤器对象已被重用（图 30-12）。

## Creating Filter Factories
过滤器可以实现 IFilterFactory 接口来负责创建过滤器的实例并指定这些实例是否可以重用。 IFilterFactory 接口定义表 30-16 中描述的成员。清单 30-35 实现了 IFilterFactory 接口并为 IsReusable 属性返回 false，以防止过滤器被重用。我使用 GetServiceOrCreateInstance 方法创建新的过滤器对象，该方法由 Microsoft.Extensions.DependencyInjection 命名空间中的 ActivatorUtilities 类定义。尽管您可以使用 new 关键字来创建过滤器，但这种方法将解决对通过过滤器的构造函数声明的服务的任何依赖性。要查看实现 IFilterFactory 接口的效果，请重新启动 ASP.NET Core 并请求 https://localhost:44350/?diag。重新加载浏览器，每次处理请求时，都会创建新的过滤器，并显示新的GUID，如图30-13所示。

## Using Dependency Injection Scopes to Manage Filter Lifecycles
过滤器可以注册为服务，这允许通过依赖注入来控制它们的生命周期，我在第 14 章中对此进行了描述。清单 30-36 将 GuidResponse 过滤器注册为范围服务。默认情况下，ASP.NET Core 为每个请求创建一个范围，这意味着将为每个请求创建一个过滤器实例。要查看效果，请重启 ASP.NET Core 并请求 https://localhost:44350/?diag。应用到 Home 控制器的两个属性都使用相同的过滤器实例进行处理，这意味着响应中的两个 GUID 是相同的。重新加载浏览器；将创建一个新的作用域，并使用一个新的过滤器对象，如图 30-14 所示。生命周期的变化在这个例子中立即生效，因为我在实现 IFilterFactory 接口时使用了 ActivatorUtilities.GetServiceOrCreateInstance 方法来创建过滤器对象。此方法将在调用其构造函数之前检查是否有可用于请求类型的服务。如果您想在不实现 IFilterFactory 和使用 ActivatorUtilities 的情况下将过滤器用作服务，您可以使用 ServiceFilter 属性应用过滤器，如下所示：ASP.NET Core 将从服务创建过滤器对象并将其应用于请求。以这种方式应用的过滤器不必从 Attribute 类派生。

# Creating Global Filters
全局过滤器应用于 ASP.NET Core 处理的每个请求，这意味着它们不必应用于单个控制器或 Razor 页面。任何过滤器都可以用作全局过滤器；但是，`action`过滤器将仅应用于端点是`action`方法的请求，而页面过滤器将仅应用于端点是 Razor 页面的请求。全局过滤器是使用 Program.cs 文件中的选项模式设置的，如清单 30-37 所示。 MvcOptions.Filters 属性返回一个集合，向其中添加过滤器以全局应用它们，使用 Add<T> 方法或对同时也是服务的过滤器使用 AddService<T> 方法。还有一个不带泛型类型参数的 Add 方法，可用于将特定对象注册为全局过滤器。清单 30-37 中的语句注册了我在本章前面创建的 HttpsOnly 过滤器，这意味着它不再需要直接应用于单个控制器或 Razor Pages，因此清单 30-38 从 Home 控制器中删除了过滤器。重新启动 ASP.NET Core 并请求 http://localhost:5000 以确认正在应用仅 HTTPS 策略，即使该属性不再用于修饰控制器。全局授权过滤器将过滤器管道短路并产生如图 30-15 所示的响应。

# Understanding and Changing Filter Order
过滤器按特定顺序运行：授权、资源、 action 或页面，然后是结果。但是，如果给定类型有多个过滤器，则应用它们的顺序由应用过滤器的范围决定。为了演示这是如何工作的，将一个名为 MessageAttribute.cs 的类文件添加到 Filters 文件夹，并使用它来定义清单 30-39 中所示的过滤器。此结果过滤器使用前面示例中所示的技术来替换来自端点的结果，并允许多个过滤器构建一系列将显示给用户的消息。清单 30-40 将 Message 过滤器的几个实例应用于 Home 控制器。清单 30-41 全局注册了 Message 过滤器。同一个过滤器有四个实例。要查看它们的应用顺序，请重新启动 ASP.NET Core 并请求 https://localhost:44350，这将产生如图 30-16 所示的响应。默认情况下，ASP.NET Core 运行全局过滤器，然后过滤器应用于控制器或页面模型类，最后过滤器应用于 action 或处理程序方法。

## Changing Filter Order
可以通过实现 IOrderedFilter 接口来更改默认顺序，ASP.NET Core 在确定如何对过滤器进行排序时会查找该接口。下面是接口的定义：Order 属性返回一个 int 值，低值的过滤器先于高 Order 值的过滤器应用。在清单 30-42 中，我在 Message 过滤器中实现了接口，并定义了一个构造函数参数，允许在应用过滤器时指定 Order 属性的值。在清单 30-43 中，我使用了构造函数参数来更改过滤器的应用顺序。顺序值可以是负数，这是确保过滤器在任何具有默认顺序的全局过滤器之前应用的有用方法（尽管您也可以在创建全局过滤器时设置顺序）。重新启动 ASP。 NET Core 并请求 https://localhost:44350 以查看新的过滤器顺序，如图 30-17 所示。
