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

# Understanding the Page Model Class
页面模型派生自 PageModel 类，该类提供 ASP.NET Core 的其余部分与 Razor 页面的视图部分之间的链接。 PageModel 类提供了管理如何处理请求的方法和提供上下文数据的属性，表 23-3 中描述了其中最有用的方法。为了完整起见，我列出了这些属性，但在 Razor 页面开发中并不经常需要它们，Razor 页面开发更侧重于选择呈现页面视图部分所需的数据。
Table 23-3. Selected PageModel Properties for Context Data
*详细内容见书本*

## Using a Code-Behind Class File
@function 指令允许将 page-behind 类和 Razor 内容定义在同一个文件中，这是流行的客户端框架（如 React 或 Vue.js）使用的一种开发方法。在同一个文件中定义代码和标记很方便，但对于更复杂的应用程序来说可能变得难以管理。 Razor 页面也可以拆分为单独的视图和代码文件，这类似于前面章节中的 MVC 示例，让人想起 ASP.NET 网页，它在称为代码隐藏文件的文件中定义 C# 类。第一步是从 CSHTML 文件中删除页面模型类，如清单 23-9 所示。我还删除了不再需要的 @using 表达式。
Listing 23-9. Removing the Page Model Class in the Index.cshtml File in the Pages Folder
```cs
@page "{id:long?}"
@model WebApp.Pages.IndexModel
<!DOCTYPE html>
<html>
    <head>
        <link href="/lib/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    </head>
    <body>
        <div class="bg-primary text-white text-center m-2 p-2">@Model.Product?.Name</div>
    </body>
</html>
```
@model 表达式已被修改以指定页面模型的命名空间，这在以前不是必需的，因为 @functions 表达式在视图的命名空间中定义了 IndexModel 类。  
在定义单独的页面模型类时，我在 WebApp.Pages 命名空间中定义该类。这不是必需的，但它使 C# 类与应用程序的其余部分保持一致。  
命名 Razor Pages 代码隐藏文件的约定是将 .cs 文件扩展名附加到视图文件的名称。
Listing 23-10. The Contents of the Index.cshtml.cs File in the Pages Folder
```cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private DataContext context;
    public Product? Product { get; set; }
    public IndexModel(DataContext ctx)
    {
        context = ctx;
    }
    public async Task OnGetAsync(long id = 1)
    {
        Product = await context.Products.FindAsync(id);
    }
}
```
### Adding a View Imports File
视图导入文件可用于避免在视图文件中使用页面模型类的完全限定名称，其作用与我在第 22 章中为 MVC 框架使用的作用相同。如果您使用的是 Visual Studio，请使用 Razor View Imports 模板将名为 _ViewImports.cshtml 的文件添加到 WebApp/Pages 文件夹，其内容如清单 23-11 所示。如果您使用的是 Visual Studio Code，请直接添加该文件。
Listing 23-11. The Contents of the _ViewImports.cshtml File in the WebApp/Pages Folder
```cs
@namespace WebApp.Pages
@using WebApp.Models
```
@namespace 指令为视图生成的 C# 类设置命名空间，并在视图导入文件中使用该指令为应用程序中的所有 Razor 页面设置默认命名空间，效果是视图及其页面模型类位于相同的命名空间中，并且 @model 指令不需要完全限定的类型，如清单 23-12 所示。
Listing 23-12. Removing the Page Model Namespace in the Index.cshtml File in the Pages Folder
```cs
@page "{id:long?}"
@model IndexModel
...
```

## Understanding Action Results in Razor Pages
虽然不是很明显，但 Razor 页面处理程序方法使用相同的 IActionResult 接口来控制它们生成的响应。为了使页面模型类更易于开发，处理程序方法具有显示页面视图部分的隐含结果。清单 23-13 明确了结果。
Listing 23-13. Using an Explicit Result in the Index.cshtml.cs File in the Pages Folder
```cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc; // <-HERE
namespace WebApp.Pages;

public class IndexModel : PageModel {
    private DataContext context;
    public Product? Product { get; set; }
    public IndexModel(DataContext ctx) {
        context = ctx;
    }
    public async Task<IActionResult> OnGetAsync(long id = 1) {  // <-HERE
        Product = await context.Products.FindAsync(id);
        return Page(); // <-HERE
    }
}
```
Page 方法继承自 PageModel 类并创建一个 PageResult 对象，该对象告诉框架呈现页面的视图部分。
与 MVC 操作方法中使用的 View 方法不同，Razor Pages 页面方法不接受参数，并且始终呈现已选择用于处理请求的页面视图部分。   
PageModel 类提供了其他方法来创建不同的操作结果以产生不同的结果，如表 23-4 中所述。
### Using an Action Result
除 Page 方法外，表 23-4 中的方法与操作方法中可用的方法相同。但是，必须小心使用这些方法，因为发送状态代码响应在 Razor 页面中没有帮助，因为它们仅在客户端需要视图内容时使用。例如，当无法找到请求的数据时，不要使用 NotFound 方法，更好的方法是将客户端重定向到另一个可以为用户显示 HTML 消息的 URL。重定向可以是静态 HTML 文件、另一个 Razor 页面或控制器定义的操作。将名为 NotFound.cshtml 的 Razor 页面添加到 Pages 文件夹。
在 `Index.cshtml.cs` 文件中添加找不到商品时的跳转：
```cs
if (Product == null) {
   return RedirectToPage("NotFound");
}
```

## Handling Multiple HTTP Methods
Razor 页面可以定义响应不同 HTTP 方法的处理程序方法。最常见的组合是支持允许用户查看和编辑数据的 GET 和 POST 方法。为了演示，将一个名为 Editor.cshtml 的 Razor 页面添加到 Pages 文件夹，将名为 Editor.cshtml.cs 的文件添加到 WebApp/Pages 文件夹。
页面模型类定义了两个处理程序方法，方法的名称告诉 Razor Pages 框架每个处理哪个 HTTP 方法。 OnGetAsync 方法用于处理 GET 请求，它通过定位一个 Product 来完成，其详细信息由视图显示。 OnPostAsync 方法用于处理 POST 请求，该请求将在用户提交 HTML 表单时由浏览器发送。 OnPostAsync 方法的参数是从请求中获取的，因此 id 值是从 URL 路由中获取的，而 price 值是从表单中获取的。 （从表单中提取数据的模型绑定特性在第 28 章中描述。）

## Selecting a Handler Method
页面模型类可以定义多个处理程序方法，允许请求使用处理程序查询字符串参数或路由段变量来选择方法。要演示此功能，请将名为 HandlerSelector.cshtml 的 Razor 页面文件添加到 Pages 文件夹。
此示例中的页面模型类定义了两个处理程序方法：OnGetAsync 和 OnGetRelatedAsync。默认使用OnGetAsync方法，重启ASP.NET Core，使用浏览器请求http://localhost:5000/handlerselector 即可看到。  
页面呈现的锚元素之一以带有处理程序查询字符串参数的 URL 为目标，如下所示：
```html
...
<a href="/handlerselector?handler=related" class="btn btn-primary">Related</a>
...
```
指定处理程序方法的名称时不带 On[method] 前缀且不带 Async 后缀，以便使用选择 OnGetRelatedAsync 方法相关的处理程序值。

# Understanding the Razor Page View
Razor 页面的视图部分使用与控制器使用的视图相同的语法并具有相同的功能。 Razor Pages 可以使用各种表达式和功能，例如会话、临时数据和布局。除了使用 @page 指令和页面模型类之外，唯一的区别是在配置布局和部分视图等功能时有一定数量的重复，如以下部分所述。
## Creating a Layout for Razor Pages
Razor 页面布局的创建方式与控制器视图相同，但位于 Pages/Shared 文件夹中。
创建 Pages/Shared 文件夹，在新文件夹中创建 _Layout.cshtml 文件。
创建 Pages/_ViewStart.cshtml 文件并添加内容。
从 Razor Pages 生成的 C# 类派生自 Page 类，该类提供视图启动文件使用的 Layout 属性，其目的与控制器视图使用的属性相同。
现在更新了Index页面以删除布局将提供的元素，只保留：
```html
@page "{id:long?}"
@model IndexModel
<div class="bg-primary text-white text-center m-2 p-2">@Model.Product?.Name</div>
```
使用视图开始文件 _ViewStart.cshtml 将布局应用于所有页面，但并不会覆盖给 Layout 属性赋值的那些页面，如 Editor.cshtml 中把 Layout 设置为 null，所以该页面不会被应用默认布局。

## Using Partial Views in Razor Pages
Razor Pages 可以使用部分视图，这样公共内容就不会重复。本节中的示例依赖于标签助手功能，我在第 25 章中详细介绍了该功能。
对于本章，将下面所示的指令添加到视图导入文件 _ViewImports.cshtml 中，它启用将自定义HTML元素应用于部分视图。
```cs
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```
在 Pages/Shared 文件夹中添加名为 _ProductPartial.cshtml 的 Razor 视图。
请注意，部分视图中没有特定于 Razor Pages 的内容。部分视图使用 @model 指令接收视图模型对象，不使用 @page 指令或具有页面模型，这两者都是特定于 Razor Pages 的。这允许 Razor Pages 与 MVC 控制器共享部分视图，如下面所述。
> **UNDERSTANDING THE PARTIAL METHOD SEARCH PATH**   
> Razor 视图引擎开始在与使用它的 Razor 页面相同的文件夹中寻找分部视图。如果没有匹配的文件，则在每个父目录中继续搜索，直到到达 Pages 文件夹。例如，对于在 Pages/App/Data 文件夹中定义的 Razor 页面使用的局部视图，视图引擎会在 Pages/App/Data 文件夹、Page/App 文件夹和 Pages 文件夹中查找。如果没有找到文件，搜索将继续到 Pages/Shared 文件夹，最后到 Views/Shared 文件夹。最后一个搜索位置允许 Razor 页面使用定义为与控制器一起使用的部分视图，这是一个有用的功能，可以避免在同时使用 MVC 控制器和 Razor 页面的应用程序中出现重复内容。

注意： 部分视图通过它们的 @model 指令接收视图模型，而不是页面模型。正是由于这个原因，model 属性的值是 Model.Product 而不仅仅是 Model。
Listing 23-25. Using a Partial View in the Index.cshtml File in the Pages Folder
```html
@page "{id:long?}"
@model IndexModel
<div class="bg-primary text-white text-center m-2 p-2">@Model.Product?.Name</div>
<partial name="_ProductPartial" model="Model.Product" />
```

## Creating Razor Pages Without Page Models
如果 Razor 页面只是简单地向用户呈现数据，则结果可能是一个页面模型类，该类仅声明构造函数依赖项以设置在视图中使用的属性。
要了解此模式，请将名为 Data.cshtml 的 Razor 页面添加到 WebApp/Pages 文件夹。
此示例 `Data.cshtml` 中的页面模型不转换数据、执行计算或执行任何操作，只是通过依赖注入为视图提供对数据的访问权限。
为了避免这种模式，其中页面模型类仅用于访问服务，@inject 指令可用于在视图中获取服务，而不需要页面模型，如 `DataWithoutPageModel.cshtml` 所示。
@inject 表达式指定服务类型和访问服务的名称。在此示例中，服务类型是 DataContext，访问它的名称是 context。在视图中，@foreach 表达式为 DataContext.Categories 属性返回的每个对象生成元素。由于此示例中没有页面模型，因此我删除了@page 和@using 指令。

#### Applying View Components in Razor Pages
Razor 页面以相同的方式使用视图组件，通过 Component 属性或通过自定义 HTML 元素。由于 Razor Pages 有自己的视图导入文件，因此需要一个单独的 @addTagHelper 指令，如清单所示。
```
@addTagHelper *, WebApp
```
