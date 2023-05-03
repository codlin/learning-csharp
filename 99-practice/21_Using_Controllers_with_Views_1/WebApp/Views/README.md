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

## Getting Context Data
有关当前请求和父视图的详细信息通过 ViewComponent 基类定义的属性提供给视图组件，如表 24-5 中所述。
Table 24-5. The ViewComponentContext Properties
| Name | Description |
|--|--|
| HttpContext | 此属性返回一个 HttpContext 对象，该对象描述当前请求和正在准备的响应。 |
| Request | 该属性返回一个描述当前 HTTP 请求的 HttpRequest 对象。 |
| User | 该属性返回一个描述当前用户的 IPrincipal 对象，如第 37 和 38 章所述。|
| RouteData | 该属性返回动态视图包对象，可用于在视图组件和视图之间传递数据，如第 22 章所述。 |
| ModelState | 此属性返回一个 ModelStateDictionary，它提供模型绑定过程的详细信息，如第 29 章所述。|
| ViewData | 此属性返回一个 ViewDataDictionary，它提供对为视图组件提供的视图数据的访问。|
可以以任何方式使用上下文数据来帮助视图组件完成其工作，包括改变选择数据的方式或呈现不同的内容或视图。
很难设计一个在视图组件中使用上下文数据的代表性示例，因为它解决的问题是特定于每个项目的。
在清单 24-19 中，我检查请求的路由数据以确定路由模式是否包含控制器段变量，该变量指示将由控制器和视图处理的请求。
Listing 24-19. Using Request Data in the CitySummary.cs File in the Components Folder
```cs
...
public string Invoke() {
    if (RouteData.Values["controller"] != null) {
        return "Controller Request";
    } else {
        return "Razor Page Request";
    }
}
...
```

### Providing Context from the Parent View Using Arguments
父视图可以为视图组件提供额外的上下文数据，为它们提供有关应生成的内容的数据或指导。上下文数据是通过 Invoke 或 InvokeAsync 方法接收的，如清单 24-20 所示。
Listing 24-20. Receiving a Value in the CitySummary.cs File in the Components Folder
```cs
...
public IViewComponentResult Invoke(string themeName)
{
    ViewBag.Theme = themeName;
    return View(new CityViewModel
    {
        Cities = data.Cities.Count(),
        Population = data.Cities.Sum(c => c.Population)
    });
}
...
```
**Using a Default Parameter Value**
可以为 Invoke 方法参数定义默认值，如清单 24-24 所示，如果父视图不提供值，它会提供回退。
```cs
public IViewComponentResult Invoke(string themeName= "success") {
    ...
}
```

### Creating Asynchronous View Components
本章到目前为止的所有示例都是同步视图组件，可以识别它们是因为它们定义了 Invoke 方法。如果您的视图组件依赖于异步 API，则您可以通过定义返回 Task 的 InvokeAsync 方法来创建异步视图组件。当 Razor 从 InvokeAsync 方法接收到 Task 时，它将等待它完成，然后将结果插入到主视图中。要创建新组件，请将名为 PageSize.cs 的类文件添加到 Components 文件夹，并使用它来定义如清单 24-26 所示的类。

## Creating View Components Classes
视图组件通常提供由控制器或 Razor 页面深入处理的功能摘要或快照。例如，对于汇总购物篮的视图组件，通常会有一个链接以控制器为目标，该控制器提供购物篮中产品的详细列表，可用于结帐和完成购买。    
在这种情况下，您可以创建一个既是视图组件又是控制器或 Razor 页面的类。请将名为 Cities.cshtml.cs 的文件添加到 Pages 文件夹。
此页面模型类使用 ViewComponent 属性进行修饰，这使其可以用作视图组件。 Name 参数指定将应用视图组件的名称。由于页面模型不能从 ViewComponent 基类继承，因此类型为 ViewComponentContext 的属性用 ViewComponentContext 属性修饰，这表明应该在 Invoke 或 InvokeAsync 方法之前为其分配一个定义表 24-5 中描述的属性的对象被调用。 View 方法不可用，所以我必须创建一个 ViewViewComponentResult 对象，它依赖于通过装饰属性接收的上下文对象。清单 24-30 更新了页面的视图部分以使用新的页面模型类。

### Creating a Hybrid Controller Class
相同的技术可以应用于控制器。将名为 CitiesController.cs 的类文件添加到 Controllers 文件夹，并添加如清单 24-33 所示的语句。
控制器实例化方式的一个怪癖意味着不需要用 ViewComponentContext 属性装饰的属性，并且从 Controller 基类继承的 ViewData 属性可用于创建视图组件结果。  
要为操作方法提供视图，请创建 Views/Cities 文件夹并向其中添加一个名为 Index.cshtml 的文件，其内容如清单 24-34 所示。
要为视图组件提供视图，请创建 Views/Shared/Components/CitiesControllerHybrid 文件夹并向其中添加一个名为 Default.cshtml 的 Razor 视图。
清单 24-36 在 Data.cshtml Razor 页面中应用了混合视图组件，替换了上一节中创建的混合类。
```html
<vc:cities-controller-hybrid />
```