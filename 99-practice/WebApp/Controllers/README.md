# 控制器说明

Controllers are classes whose methods, known as actions, can process HTTP requests.  
控制器`Controllers`是类，其方法（称为操作`actions`）可以处理 HTTP 请求。

Controllers are discovered automatically when the application is started.  
启动应用程序时会自动发现控制器。

The basic discovery process is simple: any public class whose name ends with Controller is a controller, 
and any public method a controller defines is an action.  
基本的发现过程很简单：名称以 Controller 结尾的任何公共类都是控制器，控制器定义的任何公共方法都是操作。

## 第一个控制类例子
To demonstrate how simple a controller can be, create the WebApp/Controllers folder and add to it a file named `ProductsController.cs`.  
为了演示控制器可以多么简单，创建 WebApp/Controllers 文件夹并向其中添加一个名为 `ProductsController.cs` 的文件。

## ControllerBase
控制器派生自 `ControllerBase` 类，该类提供对 MVC 框架和底层 ASP.NET Core 平台提供的功能的访问。 
表 19-4 描述了 ControllerBase 类提供的最有用的属性。 
| Name             | Description | 
|---|---|
|HttpContext        |This property returns the HttpContext object for the current request.|  
|ModelState         |This property returns details of the data validation process, as demonstrated in the “Validating Data” section later  in the chapter and described in detail in Chapter 29.  |
|Request            |This property returns the HttpRequest object for the current request.  |
|Response           |This property returns the HttpResponse object for the current response.  |
|RouteData          |This property returns the data extracted from the request URL by the routing middleware, as described in Chapter 13.  |
|User               |This property returns an object that describes the user associated with the currentrequest, as described in Chapter 38.  |

尽管控制器通常派生自 `ControllerBase` 或 `Controller` 类（在第 21 章中描述），但这只是约定，
MVC 框架将接受名称以 `Controller` 结尾的任何类，派生自名称以 `Controller` 结尾的类，或者已用 `Controller` 属性装饰。  
将 `NonController` 属性应用于满足这些条件但不应接收 HTTP 请求的类。

## Understanding the Controller Attributes
操作方法支持的 HTTP 方法和 URL 由应用于控制器的属性组合决定。控制器的 URL 由应用于类的 Route 属性指定，如下所示：  
```c#
...
[Route("api/[controller]")]
public class ProductsController: ControllerBase {
...
```
属性参数的 [controller] 部分用于从控制器类的名称派生 URL。类名的 Controller 部分被删除，这意味着`ProductsController.cs`中的属性
将控制器的 URL 设置为 `/api/products`。每个操作都装饰有一个属性，该属性指定它支持的 HTTP 方法，如下所示：
```c#
...
[HttpGet]
public Product[] GetProducts() {
...
```
操作方法命名在用于 Web 服务的控制器中无关紧要。控制器还有其他用途，名称确实很重要，但对于 Web 服务，重要的是 HTTP 方法属性和路由模式。 
HttpGet 属性告诉 MVC 框架 GetProducts 操作方法将处理 HTTP GET 请求。表 19-5 描述了可应用于操作以指定 HTTP 方法的完整属性集。
| Name             | Description | 
|---|---|
|HttpGet     |This attribute specifies that the action can be invoked only by HTTP requests that use the GET verb.|
|HttpPost    |This attribute specifies that the action can be invoked only by HTTP requests that use the POST verb.|
|HttpDelete  |This attribute specifies that the action can be invoked only by HTTP requests that use the DELETE verb.|
|HttpPut     |This attribute specifies that the action can be invoked only by HTTP requests that use the PUT verb.|
|HttpPatch   |This attribute specifies that the action can be invoked only by HTTP requests that use the PATCH verb.|
|HttpHead    |This attribute specifies that the action can be invoked only by HTTP requests that use the HEAD verb. |
|AcceptVerbs |This attribute is used to specify multiple HTTP verbs.|

应用于操作以指定 HTTP 方法的属性也可用于构建控制器的基本 URL。
```c#
...
[HttpGet("{id}")]
public Product GetProduct() {
...
```
此属性告诉 MVC 框架，GetProduct 操作方法处理对 URL 模式 api/products/{id} 的 GET 请求。在发现过程中，应用于控制器的属性用于构建控制器可以处理的 URL 模式集，如表 19-6 所示。
| HTTP Method | URL Pattern | Action Method Name | 
|---|---|---|
| GET | api/products      | GetProducts |
| GET | api/products/{id} | GetProduct  |

## Using Asynchronous Actions
ASP.NET Core 平台通过从池中分配一个线程来处理每个请求。可以并发处理的请求数受池大小的限制，线程在等待操作产生结果时不能用于处理任何其他请求。
依赖于外部资源的操作可能会导致请求线程等待较长时间。例如，数据库服务器可能有自己的并发限制，并且可能会在查询可以执行之前排队查询。
在数据库为操作生成结果之前，ASP.NET Core 请求线程无法处理任何其他请求，然后生成可发送到 HTTP 客户端的响应。
这个问题可以通过定义异步操作来解决，它允许 ASP.NET Core 线程在其他请求被阻止时处理其他请求，从而增加应用程序可以同时处理的 HTTP 请求的数量。  
`ProductsController.cs` 修改了控制器以使用异步操作。
Entity Framework Core 提供了一些方法的异步版本，例如 FindAsync、AddAsync 和 SaveChangesAsync，我将这些方法与 await 关键字一起使用。
并非所有操作都可以异步执行，这就是 UpdateProduct 和 DeleteProduct 操作中的 Update 和 Remove 方法保持不变的原因。对于某些操作（包括对数据库的 LINQ 查询），可以使用 IAsyncEnumerable<T> 接口，它表示应该异步枚举的对象序列，并防止 ASP.NET Core 请求线程等待每个对象生成数据库。控制器产生的响应没有变化，但 ASP.NET Core 分配用于处理每个请求的线程不一定被操作方法阻塞。

## Preventing Over-Binding
默认情况下，Entity Framework Core 将数据库配置为在存储新对象时分配主键值。这意味着应用程序不必担心跟踪哪些键值已经分配，​​并且允许多个应用程序共享同一个数据库而无需协调键分配。 Product 数据模型类需要一个 ProductId 属性，但模型绑定过程不理解该属性的重要性，并将客户端提供的任何值添加到它创建的对象中，这导致 SaveProduct 操作方法中出现异常。  
这被称为过度绑定`over-binding`，当客户端提供开发人员未预料到的值时，它会导致严重的问题。充其量，应用程序的行为会出乎意料，但这种技术已被用来破坏应用程序的安全性，并授予用户比他们应有的更多访问权限。防止过度绑定的最安全方法是创建单独的数据模型类，这些类仅用于通过模型绑定过程接收数据。将名为 ProductBindingTarget.cs 的类文件添加到 WebApp/Models 文件夹，并使用它来定义类。

## Using Action Results
ASP.NET Core 会自动设置响应的状态代码，但您并不总能得到想要的结果，部分原因是没有针对 RESTful Web 服务的严格规则，并且 Microsoft 做出的假设可能与您的期望不符。一个例子是请求product，请求的 URL 将由 GetProduct 操作方法处理，它将在数据库中查询 ProductId 值为 1000 的对象，命令产生以下输出：`数据库中没有匹配的对象`，这意味着GetProduct 操作方法返回 null。当 MVC 框架从操作方法接收到 null 时，它会返回 204 状态代码，这表示成功的请求没有产生任何数据。并非所有 Web 服务都以这种方式运行，常见的替代方法是返回 404 响应，表示未找到。类似地，SaveProducts 操作在存储对象时将返回 200 响应，但由于在存储数据之前不会生成主键，因此客户端不知道分配的键值是什么。对于这些类型的 Web 服务实现细节，这里没有对错之分，您应该选择最适合您的项目和个人喜好的方法。本节是如何更改默认行为的示例，而不是遵循任何特定样式的 Web 服务的方向。  
动作方法可以指示 MVC 框架通过返回一个实现 IActionResult 接口的对象来发送特定的响应，这被称为动作结果。这允许操作方法指定所需的响应类型，而不必使用 HttpResponse 对象直接生成它。ControllerBase 类提供了一组用于创建操作结果对象的方法(OK, BadRequest, NotFound等等)，这些对象可以从操作方法返回。
```C#
[HttpGet("{id}")]
public async Task<IActionResult> GetProduct(long id) {
    Product? p = await context.Products.FindAsync(id);
    if (p == null) {
        return NotFound();
    }
    return Ok(p);
}

[HttpPost]
public async Task<IActionResult>
SaveProduct([FromBody] ProductBindingTarget target) {
    Product p = target.ToProduct();
    await context.Products.AddAsync(p);
    await context.SaveChangesAsync();
    return Ok(p);
}
```
当一个action方法返回一个对象时，相当于把这个对象传递给Ok方法并返回结果。当操作返回 null 时，它相当于返回 NoContent 方法的结果。

### Performing Redirections
表 19-7 中的许多操作结果方法都与重定向有关，将客户端重定向到另一个 URL。执行重定向的最基本方法是调用 Redirect 方法，如清单 19-21 所示
```c#
[HttpGet("redirect")]
public IActionResult Redirect() {
    return Redirect("/api/products/1");
}
```

### Redirecting to an Action Method
您可以使用 RedirectToAction 方法（用于临时重定向）或 RedirectToActionPermanent 方法（用于永久重定向）重定向到另一个操作方法。
清单 19-23 更改了 Redirect 操作方法，以便客户端将被重定向到控制器定义的另一个操作方法。
```C#
...
[HttpGet("redirect")]
public IActionResult Redirect() {
    return RedirectToAction(nameof(GetProduct), new { Id = 1 });
}
...
```
操作方法被指定为字符串，尽管 nameof 表达式可用于选择操作方法而没有拼写错误的风险。使用匿名对象提供创建路由所需的任何其他值。  
如果您仅指定一个操作方法名称，则重定向将以当前控制器为目标。有一个 RedirectToAction 方法的重载可以接受控制器名称和操作Action。  
#### REDIRECTING USING ROUTE VALUES 
RedirectToRoute 和 RedirectToRoutePermanent 方法将客户端重定向到一个 URL，该 URL 通过为路由系统提供段变量的值并允许它选择要使用的路由而创建。
这对于具有复杂路由配置的应用程序很有用，应谨慎使用，因为很容易创建到错误 URL 的重定向。下面是使用 RedirectToRoute 方法进行重定向的示例： 
```C#
...
[HttpGet("redirect")]
public IActionResult Redirect() {
    return RedirectToRoute(new {
        controller = "Products", action = "GetProduct", Id = 1
    });
}
...
```
此重定向中的值集依赖于约定路由来选择控制器和操作方法。约定路由通常与生成 HTML 响应的控制器一起使用，如第 21 章所述。

### Applying the API Controller Atribute
ApiController 属性可应用于 Web 服务控制器类，以更改模型绑定和验证功能的行为。
使用 FromBody 属性从请求正文中选择数据并显式检查 ModelState.IsValid 属性在已使用 ApiController 属性装饰的控制器中不是必需的。
从主体获取数据和验证数据在 Web 服务中非常普遍，因此在使用属性时会自动应用它们，将控制器操作中的代码重点恢复到处理应用程序功能，如清单所示.
```C#
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase {
    ...
    [HttpPost]
    public async Task<IActionResult>
    SaveProduct(ProductBindingTarget target) {
        Product p = target.ToProduct();
        await context.Products.AddAsync(p);
        await context.SaveChangesAsync();
        return Ok(p);
    }
    ...
}
```
### Omitting Null Properties
我将在本章中进行的最后更改是从 Web 服务返回的数据中删除空值。
数据模型类包含 Entity Framework Core 用于关联复杂查询中的相关数据的导航属性，如第 20 章中所述。
对于本章中执行的简单查询，没有值分配给这些导航属性，这意味着客户端收到的属性的值永远不会可用。
要查看问题，请使用 PowerShell 命令提示符运行清单 19-28 中所示的命令。
```POWERSHELL
Invoke-WebRequest http://localhost:5000/api/products/1 | Select-Object Content
```
结果：
```POWERSHELL
Content
-------
{"productId":1,"name":"Green Kayak","price":275.00,"categoryId":1,"category":null,
"supplierId":1,"supplier":null}
```

#### Projecting Selected Properties
```c#
[HttpGet("{id}")]
    public async Task<IActionResult?> GetProduct(long id, [FromServices] ILogger<ProductsController> logger)
    {
        logger.LogDebug("GetProduct Action Invoke");
        Product? p = await context.Products.FindAsync(id);
        if (p == null)
        {
            return NotFound();
        }
        return Ok(new
        {
            // projecting Selected Properties
            ProductId = p.ProductId,
            Name = p.Name,
            Price = p.Price,
            CategoryId = p.CategoryId,
            SupplierId = p.SupplierId
        });
    }
```
result:
```json
Content
-------
{"productId":1,"name":"Green Kayak","price":275.00,"categoryId":1,"supplierId":1}
```
