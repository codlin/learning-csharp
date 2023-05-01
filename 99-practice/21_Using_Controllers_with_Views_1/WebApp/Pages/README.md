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

### Understanding the Page View
Razor 页面使用相同的 HTML 片段和代码表达式组合来生成内容，这些内容定义了呈现给用户的视图。页面模型的方法和属性可通过 @Model 表达式在 Razor 页面中访问。 IndexModel 类定义的 Product 属性用于设置 HTML 元素的内容，如下所示：
```cs
<div class="bg-primary text-white text-center m-2 p-2">@Model.Product?.Name</div>
```
@Model 表达式返回一个 IndexModel 对象，这个表达式读取 Product 属性返回的对象的 Name 属性。 Model 属性不需要 null 条件运算符 (?)，因为它将始终分配给页面模型类的实例并且不能为 null。但是，页面模型类定义的属性可以为 null，这就是我在 Razor 表达式中使用 Product 属性运算符的原因。

### Understanding the Generated C# Class
在幕后，Razor 页面被转换为 C# 类，就像常规 Razor 视图一样。
如果将此代码与第 21 章中显示的等效代码进行比较，您可以看到 Razor Pages 如何依赖 MVC 框架使用的相同功能。 HTML 片段和视图表达式被转换为对 WriteLiteral 和 Write 方法的调用。

# Understanding Razor Pages Routing
Razor Pages 依赖 CSHTML 文件的位置进行路由，因此对 http://localhost:5000/index 的请求由 Pages/Index.cshtml 文件处理。为应用程序添加更复杂的 URL 结构是通过添加其名称代表您要支持的 URL 中的段的文件夹来完成的。例如，创建 WebApp/Pages/Suppliers 文件夹并向其中添加一个名为 List.cshtml 的 Razor 页面，其内容如 `Pages/Suppliers/List.cshtml` 所示，则其路由为 `http://localhost:5000/suppliers/list` 。

## UNDERSTANDING THE DEFAULT URL HANDLING
MapRazorPages 方法为 Index.cshtml Razor 页面的默认 URL 设置路由，遵循 MVC 框架使用的类似约定。正是出于这个原因，添加到项目中的第一个 Razor 页面通常称为 Index.cshtml。但是，当应用程序将 Razor Pages 和 MVC 框架混合在一起时，Razor Pages 定义的默认路由优先，因为它是用较低的顺序创建的（路由顺序在第 13 章中描述）。这意味着请求 http://localhost:5000 由示例项目中的 Index.cshtml Razor 页面处理，而不是 Home 控制器上的 Index 操作。  
如果您希望 MVC 框架处理默认 URL，则可以更改分配给 Razor Pages 路由的顺序，如下所示：
```cs
app.MapRazorPages().Add(b => ((RouteEndpointBuilder)b).Order = 2);
```
Razor Pages 路由是使用 0 的顺序创建的，这使它们优先于使用 1 的顺序创建的 MVC 路由。分配 2 的顺序使 MVC 框架路由优先。  
在我自己的项目中，我混合了 Razor 页面和 MVC 控制器，我倾向于依赖 MVC 框架来处理默认 URL，并且我避免创建 Index.cshtml Razor 页面以避免混淆。

## Specifying a Routing Pattern in a Razor Page
使用文件夹和文件结构来执行路由意味着没有段变量供模型绑定过程使用。相反，请求处理程序方法的值是从 URL 查询字符串中获取的，您可以通过使用浏览器请求 http://localhost:5000/index?id=2 来查看。
请求 URL 中的查询字符串参数用于在调用 OnGetAsync 方法时提供 id 参数，该方法用于查询数据库中的产品。 
```cs
public async Task OnGetAsync(long id = 1) { ... }
```
@page 指令可以与路由模式一起使用，它允许定义段变量，如清单 23-6 所示。
Listing 23-6. Defining a Segment Variable in the Index.cshtml File in the Pages Folder
```cs
@page "{id:long?}"
```
我们除了使用查询字符串的访问访问外，现在还可以用 `http://localhost:5000/index/2` 的URL形式来访问了。
同样的，我们使用 @page 指令替换 Razor 页面的默认基于文件的路由，修改List.cshtml来使用段变量访问：
```cs
@page "/lists/suppliers"
```
值得注意的是，我们指定了page的访问url后，就不能再用约定路由的方式访问了。但我们也可以通过配置语句为页面配置一个或多个路由，见下一节。

## Adding Routes for a Razor Page
使用 @page 指令替换 Razor 页面的默认基于文件的路由。如果要为一个页面定义多个路由，则可以将配置语句添加到 Program.cs 文件中，如清单所示：
```cs
using Microsoft.AspNetCore.Mvc.RazorPages;
...
builder.Services.Configure<RazorPagesOptions>(opts => {
    opts.Conventions.AddPageRoute("/Index", "/extra/page/{id:long?}");
});
```
