# Using View Components
1. 视图组件是什么？
视图组件是提供应用程序逻辑以支持部分视图或将 HTML 或 JSON 数据的小片段注入父视图的类。
2. 视图组件为什么有用？
如果没有视图组件，就很难以易于维护的方式创建嵌入式功能，例如购物篮或登录面板。
3. 视图组件怎么用？
视图组件通常派生自 ViewComponent 类，并使用自定义 vc HTML 元素或 @await Component.InvokeAsync 表达式应用于父视图。

## Understanding View Components
应用程序通常需要在与应用程序的主要目的无关的视图中嵌入内容。常见示例包括站点导航工具和身份验证面板，它们使用户无需访问单独的页面即可登录。  
此类功能的数据不是从操作方法或页面模型传递到视图的模型数据的一部分。正是出于这个原因，我在示例项目中创建了两个数据源：我将显示一些使用 City 数据生成的内容，这在从 Entity Framework Core 存储库和它包含的产品、类别和供应商对象。  
Partial views 用于创建视图中所需的可重用标记，避免在应用程序的多个位置复制相同的内容。Partial views 是一个有用的功能，但它们只包含 HTML 片段和 Razor 指令，并且它们操作的数据是从父视图接收的。如果您需要显示不同的数据，就会遇到问题。您可以直接从局部视图访问您需要的数据，但这会破坏开发模型并产生难以理解和维护的应用程序。或者，您可以扩展应用程序使用的视图模型，以便它包含您需要的数据，但这意味着您必须更改每个操作方法，这使得很难隔离操作方法的功能以进行有效的维护和测试。   
这就是视图组件的用武之地。视图组件是一个 C# 类，它提供具有所需数据的部分视图，独立于操作方法或 Razor 页面。在这方面，视图组件可以被认为是一种专门的操作或页面，但它仅用于提供带有数据的部分视图；它不能接收 HTTP 请求，它提供的内容将始终包含在父视图中。

## Creating and Using a View Component
视图组件是名称以 ViewComponent 结尾并定义 Invoke 或 InvokeAsync 方法的任何类，或者是派生自 ViewComponent 基类或已使用 ViewComponent 属性修饰的任何类。我在“获取上下文数据”部分演示了属性的使用，但本章中的其他示例依赖于基类。    
视图组件可以在项目的任何地方定义，但约定是将它们分组在一个名为 Components 的文件夹中。创建 WebApp/Components 文件夹并向其中添加一个名为 CitySummary.cs 的类文件。    
视图组件可以利用依赖注入来接收它们需要的服务。

### Applying a View Component
可以通过两种不同的方式应用视图组件。    
第一种技术是使用`组件Component`属性，该属性已经添加到 `Views` 和 `Razor Pages` 生成的 C# 类中。此属性返回一个实现 IViewComponentHelper 接口的对象，该接口提供 InvokeAsync 方法。清单使用此技术在 Views/Home 文件夹中的 Index.cshtml 文件中应用视图组件。
```cs
@section Summary {
    <div class="bg-info text-white m-2 p-2">
        @await Component.InvokeAsync("CitySummary")
    </div>
}
```
使用 Component.InvokeAsync 方法应用视图组件，使用视图组件类的名称作为参数。这种技术的语法可能令人困惑。视图组件类定义 Invoke 或 InvokeAsync 方法，具体取决于它们的工作是同步执行还是异步执行。但是始终使用 Component.InvokeAsync 方法，甚至应用定义了 Invoke 方法且其操作完全同步的视图组件。    
为了将视图组件的命名空间添加到视图中包含的列表中，我将清单中所示的语句添加到 Views 文件夹中的 _ViewImports.cshtml 文件中。
```cs
@using WebApp.Components
```
#### Applying View Components Using a Tag Helper
Razor 视图和页面可以包含标记帮助器`Tag Helpers`，它们是由 C# 类管理的自定义 HTML 元素。我在第 25 章详细解释了标签助手的工作原理，但是可以使用作为标签助手实现的 HTML 元素来应用视图组件。要启用此功能，请将清单 24-10 中所示的指令添加到 Views 文件夹中的 _ViewImports.cshtml 文件中。  
注意：视图组件只能在控制器视图或 Razor 页面中使用，不能直接用于处理请求。
Configuring a Tag Helper in the _ViewImports.cshtml File in the Views Folder
```cs
...
@addTagHelper *, WebApp
```
自定义元素的标签是 vc，后跟一个冒号，然后是视图组件类的名称，它被转换为 kebab-case。类名中的每个大写单词都被转换为小写并用连字符分隔，这样 CitySummary 就变成了 city-summary，并且使用 vc:city-summary 元素应用了 CitySummary 视图组件。

#### Applying View Components in Razor Pages
Razor 页面以相同的方式使用视图组件，通过 Component 属性或通过自定义 HTML 元素。由于 Razor Pages 有自己的视图导入文件，因此需要一个单独的 @addTagHelper 指令，加入到`Pages/_ViewImports.cshtml`中，如清单所示。
```
@addTagHelper *, WebApp
```
Using a View Component in the Data.cshtml File in the Pages Folder
```html
<div class="bg-info text-white m-2 p-2">
    <vc:city-summary />
</div>
```

## Understanding View Component Results
将简单字符串值插入视图或页面的能力并不是特别有用，但幸运的是，视图组件的功能要多得多。
通过让 Invoke 或 InvokeAsync 方法返回一个实现 IViewComponentResult 接口的对象，可以实现更复杂的效果。
表 24-3 描述了三个实现 IViewComponentResult 接口的内置类，以及 ViewComponent 基类提供的创建它们的便捷方法。
我将在接下来的部分中描述每种结果类型的使用。
| Name | Description |
|--|--|
| ViewViewComponentResult | 此类用于指定具有可选视图模型数据的 Razor 视图。此类的实例是使用 View 方法创建的。 |
| ContentViewComponentResult | 此类用于指定将被安全编码以包含在 HTML 文档中的文本结果。此类的实例是使用 Content 方法创建的。 |
| HtmlContentViewComponentResult | 此类用于指定将包含在 HTML 文档中而无需进一步编码的 HTML 片段。没有ViewComponent 方法来创建此类结果。|
有两种结果类型的特殊处理。如果一个视图组件返回一个字符串，那么它将用于创建一个 ContentViewComponentResult 对象，这是我在前面的示例中所依赖的。如果视图组件返回 IHtmlContent 对象，则它用于创建 HtmlContentViewComponentResult 对象。

### Returning a Partial View
最有用的响应是命名笨拙的 ViewViewComponentResult 对象，它告诉 Razor 呈现局部视图并将结果包含在父视图中。 ViewComponent 基类提供了用于创建 ViewViewComponentResult 对象的 View 方法，该方法有四个版本可用，如表 24-4 所述。
| Name | Description |
|--|--|
| View() | Using this method selects the default view for the view component and does not provide a view model. |
| View(model) | Using the method selects the default view and uses the specified object as the view model. |
| View(viewName) | Using this method selects the specified view and does not provide a view model. |
| View(viewName, model) |  Using this method selects the specified view and uses the specified object as the view model. |
这些方法对应于 Controller 基类提供的方法，并且使用方式大致相同。要创建视图组件可以使用的视图模型类，请将名为 CityViewModel.cs 的类文件添加到 WebApp/Models 文件夹，并使用它来定义如清单 24-14 所示的类。
Listing 24-14. The Contents of the CityViewModel.cs File in the Models Folder
```cs
namespace WebApp.Models;

public class CityViewModel
{
    public int? Cities { get; set; }
    public int? Population { get; set; }
}
```
清单 24-15 修改了 CitySummary 视图组件的 Invoke 方法，因此它使用 View 方法选择部分视图并使用 CityViewModel 对象提供视图数据。
```cs
...
public IViewComponentResult Invoke()
{
    return View(new CityViewModel
    {
        Cities = data.Cities.Count(),
        Population = data.Cities.Sum(c => c.Population)
    });
}
...
```
创建 WebApp/Views/Shared/Components/CitySummary 文件夹并向其中添加一个名为 Default.cshtml 的 Razor 视图。

### Returning HTML Fragments
ContentViewComponentResult 类用于在不使用视图的情况下在父视图中包含 HTML 片段。 ContentViewComponentResult 类的实例是使用从 ViewComponent 基类继承的 Content 方法创建的，它接受一个字符串值。清单 24-17 演示了 Content 方法的使用。
Listing 24-17. Using the Content Method in the CitySummary.cs File in the Components Folder
```cs
public IViewComponentResult Invoke() {
    return Content("This is a <h3><i>string</i></h3>");
}
```
Content 方法接收到的字符串经过编码，可以安全地包含在 HTML 文档中。这在处理用户或外部系统提供的内容时尤为重要，因为它可以防止 JavaScript 内容嵌入到应用程序生成的 HTML 中。  
如果您查看视图组件生成的 HTML，您会看到尖括号已被替换，以便浏览器不会将内容解释为 HTML 元素，如下所示： 
```html
...
<div class="bg-info text-white m-2 p-2">
    This is a &lt;h3&gt;&lt;i&gt;string&lt;/i&gt;&lt;/h3&gt;
</div>
...
```
如果您信任，则不需要对内容进行编码它的来源，并希望它被解释为 HTML。 Content 方法总是对其参数进行编码，因此您必须直接创建 HtmlContentViewComponentResult 对象并为其构造函数提供一个 HtmlString 对象，该对象表示您知道可以安全显示的字符串，因为它来自您信任的源或因为您确信它已经被编码，如清单 24-18 所示。
```cs
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Html;
...
public IViewComponentResult Invoke() {
    return new HtmlContentViewComponentResult(
        new HtmlString("This is a <h3><i>string</i></h3>"));  // <-HERE
}
...
```
这种技术应该谨慎使用，并且只能用于不能被篡改并执行自己编码的内容源。
