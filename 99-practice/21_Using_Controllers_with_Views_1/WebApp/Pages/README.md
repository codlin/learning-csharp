# Understanding Razor Pages
当您了解 Razor Pages 的工作原理时，您将看到它们与 MVC 框架共享功能。事实上，Razor Pages 通常被描述为 MVC 框架的简化——这是事实——但这并不能说明 Razor Pages 为什么有用。   
MVC 框架以相同的方式解决所有问题：控制器定义选择视图以产生响应的操作方法。这是一个有效的解决方案，因为它非常灵活：控制器可以定义多个响应不同请求的操作方法，操作方法可以决定在处理请求时使用哪个视图，视图可以依赖于私有或分享部分观点以产生回应。  
并非 Web 应用程序中的每个功能都需要 MVC 框架的灵活性。对于许多功能，将使用单个操作方法来处理范围广泛的请求，所有这些请求都使用相同的视图进行处理。 Razor Pages 提供了一种更加集中的方法，将标记和 C# 代码联系在一起，牺牲了集中的灵活性。  
但是 Razor Pages 有局限性。 Razor Pages 开始时倾向于专注于单一功能，但随着功能的增强逐渐失去控制。而且，与 MVC 控制器不同，Razor Pages 不能用于创建 Web 服务。  
您不必只选择一种模型，因为 MVC 框架和 Razor Pages 共存，如本章所示。这意味着可以使用 Razor Pages 轻松开发自包含功能，而将应用程序的更复杂方面留给使用 MVC 控制器和操作来实现。

## Configuring Razor Pages
要为 Razor Pages 准备应用程序，必须将语句添加到 Program.cs 文件以设置服务和配置端点路由系统，如清单 23-3 所示。
```cs
...
builder.Services.AddRazorPages();
...
app.MapRazorPages();
```
## Creating a Razor Page
参见 `Pages/index.cshtml`

### Understanding the URL Routing Convention
Razor Pages 的 URL 路由基于文件名和相对于 Pages 文件夹的位置。`Pages/index.cshtml` 中的 Razor 页面位于 Pages 文件夹中名为 Index.cshtml 的文件中，这意味着它将处理对 /index 的请求。可以覆盖路由约定，如“了解 Razor 页面路由”部分中所述，但默认情况下，Razor 页面文件的位置决定了它响应的 URL。

### Understanding the Page Mode
在 Razor 页面中，@model 指令用于选择页面模型类，而不是标识操作方法提供的对象的类型。`Pages/index.cshtml` 中的 @model 指令选择了 IndexModel 类。
```cshtml
...
@model IndexModel
...
```
页面模型在 @functions 指令中定义，并派生自 PageModel 类，如下所示：
```cs
...
@functions {
    public class IndexModel: PageModel {
...
```
选择Razor Page以处理HTTP请求时，创建了页面模型类的新实例，并使用第14章中描述的功能使用依赖项注入来解决使用构造函数参数声明的任何依赖项。
IndexModel 类声明了对第 18 章中创建的 DataContext 服务的依赖，这允许它访问数据库中的数据。
```cs
public IndexModel(DataContext ctx) {
    context = ctx;
}
```
创建页面模型对象后，将调用处理程序方法。处理程序方法的名称是 On，后跟请求的 HTTP 方法，以便在选择 Razor 页面处理 HTTP GET 请求时调用 OnGet 方法。处理程序方法可以是异步的，在这种情况下，GET 请求将调用 OnGetAsync 方法，这是由 IndexModel 类实现的方法。
```cs
public async Task OnGetAsync(long id = 1) {
    Product = await context.Products.FindAsync(id);
}
```
处理程序方法参数的值是使用模型绑定过程从 HTTP 请求中获取的，这在第 28 章中有详细描述。OnGetAsync 方法从模型绑定器接收其 id 参数的值，它用于查询数据库和将结果分配给其 Product 属性。