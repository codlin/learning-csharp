
## Using Shared Views
当 Razor 视图引擎找到一个视图时，它会在 View/[controller] 文件夹中查找，然后在 Views/Shared 文件夹中查找。
这种搜索模式意味着包含公共内容的视图可以在控制器之间共享，避免重复。

# Working with Layouts
示例应用程序中的视图包含处理设置 HTML 文档、定义 head 部分、加载 Bootstrap CSS 文件等的重复元素。   
Razor 支持布局，将常用内容合并到一个文件中，供任何视图使用。布局通常存储在 Views/Shared 文件夹中，因为它们通常由多个控制器的操作方法使用。  
在 Views/Shared 文件夹中创建一个名为 _Layout.cshtml 的文件。
**布局包含将由多个视图使用的公共内容。通过调用 RazorPage<T> 类继承的 RenderBody 方法将每个视图独有的内容插入到响应中**。  
使用布局的视图可以仅关注其独特的内容，如图所示在清单 22-10 中。
Listing 22-10. Using a Layout in the Index.cshtml File in the Views/Home Folder
```cs
@model Product
@{
    Layout = "_Layout";
}
<div class="m-2">
   ... 
</div>
```

## Using a View Start File
你不必在每一个视图中设置 `Layout` 属性，只需要添加一个 `view start` 文件作为默认的 `Layout` 值。把 `_ViewStart.cshtml` 添加到 `Views` 目录中。
`_ViewStart.cshtml` 中设置了 `Layout` 属性的值，并把它的值作为默认的值应用与其它视图。

## Overriding the Default Layout
有两种情况你需要在视图中定义布局Layout属性，即使已经设置了view start文件。
第一种情况是一个视图需要不同的布局。
第二种可能需要 Layout 属性的情况是当视图包含完整的 HTML 文档并且根本不需要布局时。此时可以在视图中把Layout 设置为 null;

## Using Layout Sections
Razor 视图引擎支持部分的概念，它允许您在布局中提供内容区域。 Razor sections 可以更好地控制将视图的哪些部分插入到布局中以及它们的放置位置。
Section是使用 Razor @section 表达式定义的，后跟section的名称。_Layout.cshtml 定义了名为 Header 和 Footer 的部分，这些部分可以包含相同的 HTML 内容和表达式组合，就像视图的主要部分一样。使用@RenderSection 表达式在布局中应用部分。
### Using Optional Layout Sections
所有定义的section都必须在视图渲染时能够找得到，否则会抛出异常。譬如在 `_Layout.cshtml` 中定义的 `@RenderSection("Header")`，如果包含了这个布局的视图中没有关于 `@section Header` 的定义，那么该视图抛出异常。可以在定义section时把它定义为可选的(在后面加上false，如 `@RenderSection("Header"， false)`)，这样就不会存在此类问题。
### Testing for Layout Sections
IsSectionDefined 方法用于确定视图是否定义了指定的部分，并且可以在 if 表达式中用于呈现回退内容。`_Layout.cshtml` 中呈现了此类用法。

## Using Partial Views
您经常需要在几个不同的地方使用同一组 HTML 元素和表达式。部分视图是包含内容片段的视图，这些内容片段将包含在其他视图中以产生复杂的响应而不会重复。
### Enabling Partial Views
部分视图使用称为标签助手`tag helpers`的功能来应用，第 25 章对此进行了详细描述；标签助手在视图导入文件中配置，请将清单中所示的语句添加到 _ViewImports.cshtml 文件中。
```cs
@using WebApp.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```
### Creating a Partial View
### Selecting the Partial View Model Using an Expression
for 属性用于使用*应用于视图模型的表达式来*设置部分视图的模型，这是一个比描述更容易展示的特性。
在 Views/Home 文件夹中的 _RowPartial.cshtml 文件中使用分部视图：
```cs
@model Product
<tr>
    <partial name="_CellPartial" for="Name" />
    <td>@Model?.Price</td>
</tr>
```
Views/Home 文件夹中 _CellPartial.cshtml 文件的内容：
```c#
@model string
<td class="bg-info text-white">@Model</td>
```

## Understanding Content-Encoding
Razor 视图为编码内容提供了两个有用的功能。 HTML 内容编码功能可确保表达式响应不会更改发送到浏览器的响应结构，这是一项重要的安全功能。 JSON 编码功能将对象编码为 JSON 并将其插入到响应中，这是一个有用的调试功能，在向 JavaScript 应用程序提供数据时也很有用。这两种编码功能都在以下部分中进行了描述。
### Understanding HTML Encoding
Razor View 引擎对表达式结果进行编码，使它们可以安全地包含在 HTML 文档中，而无需更改其结构。在处理用户提供的内容时，这是一个重要的功能，用户可能会试图破坏应用程序或意外输入危险内容。清单 22-30 向 Home 控制器添加了一个Html()操作方法，该方法将 HTML 片段传递给 View 方法。
Listing 22-30. Adding an Action in the HomeController.cs File in the Controllers Folder
```cs
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

public class HomeController : Controller {
    private DataContext context;

    public HomeController(DataContext ctx) {
        context = ctx;
    }

    public async Task<IActionResult> Index(long id = 1) {
        ViewBag.AveragePrice =
            await context.Products.AverageAsync(p => p.Price);
        return View(await context.Products.FindAsync(id));
    }
    public IActionResult List() {
        return View(context.Products);
    }
    public IActionResult Html() {
        return View((object)"This is a <h3><i>string</i></h3>");
    }
}
```
Html() 方法中的内容将会被安全编码，在其视图中将显示为纯文本。如果要显示为html元素，则可以在视图Html.cshtml中调用Html.Raw方法。
```cs
<div class="bg-secondary text-white text-center m-2 p-2">@Html.Raw(Model)</div>
```
**警告** 除非您完全确信不会将恶意内容传递到视图，否则不要禁用安全编码。粗心地使用此功能会给您的应用程序和用户带来安全风险。
