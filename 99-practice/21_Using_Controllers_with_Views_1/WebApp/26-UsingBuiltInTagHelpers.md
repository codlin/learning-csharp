# Using the Built-in Tag Helpers

ASP.NET Core 提供了一组内置的标签助手，用于应用最常需要的元素转换。在本章中，我将解释那些处理锚点、脚本、链接和图像元素的标签助手，以及缓存内容和根据环境选择内容的功能。在第 27 章中，我描述了支持 HTML 表单的标签助手。表 26-1 将内置的标签助手放在上下文中。
| Question | Answer |
|-|-|
| 它们是什么？     | 内置的标签助手对 HTML 元素执行通常需要的转换。                |
| 为什么它们有用？  | 使用内置的标签助手意味着你不必使用第 25 章中的技术创建自定义助手。|
| 如何用它们？     | 使用标准 HTML 元素上的属性或通过自定义 HTML 元素应用标签助手。  |
| 有那么坑或者限制？| 没有，这些标签助手经过充分测试且易于使用。除非您有特殊需求，否则使用这些标签助手比自定义实现更可取。|
| 有没有其他选择？  | 这些标签助手是可选的，它们的使用不是必需的。|

## Enabling the Built-in Tag Helpers
内置标签助手全部在 Microsoft.AspNetCore.Mvc.TagHelpers 命名空间中定义，并通过将 @addTagHelpers 指令添加到各个视图或页面来启用，或者，如在示例项目的情况下，查看导入文件。这是 Views/Pages 文件夹中 _ViewImports.cshtml 文件中的必需指令，它为控制器视图启用内置标签助手： 
```cs
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```
这些指令已添加到第 24 章的示例项目中以启用视图组件功能。

## Transforming Anchor Elements
a 元素是在应用程序中导航和向应用程序发送 GET 请求的基本工具。 AnchorTagHelper 类用于转换元素的 href 属性，以便它们以使用路由系统生成的 URL 为目标，这意味着不需要硬编码 URL，路由配置的更改将自动反映在应用程序的锚元素中。表 26-3 描述了 AnchorTagHelper 类支持的属性。
Table 26-3. The Built-in Tag Helper Attributes for Anchor Elements
| Name | Description |
|-|-|
| asp-action | This attribute specifies the action method that the URL will target.|
| asp-controller | This attribute specifies the controller that the URL will target. If this attribute is omitted, then the URL will target the controller or page that rendered the current view.|
| asp-page | This attribute specifies the Razor Page that the URL will target.|
| asp-page-handler | This attribute specifies the Razor Page handler function that will process the request, as described in Chapter 23.|
| asp-fragment | This attribute is used to specify the URL fragment (which appears after the # character).|
| asp-host | This attribute specifies the name of the host that the URL will target.|
| asp-protocol | This attribute specifies the protocol that the URL will use.|
| asp-route | This attribute specifies the name of the route that will be used to generate the URL.|
| asp-route-* | Attributes whose name begins with asp-route- are used to specify additional values for the URL so that the asp-route-id attribute is used to provide a value for the id segment to the routing system.|
| asp-all-route-data | This attribute provides values used for routing as a single value, rather than using individual attributes.|

AnchorTagHelper 简单且可预测，可以轻松地在使用应用程序路由配置的元素中生成 URL。清单 26-7 添加了一个锚点元素，该元素使用表中的属性创建一个 URL，该 URL 以 Home 控制器定义的另一个操作为目标。
Listing 26-7. Transforming an Element in the _RowPartial.cshtml File in the Views/Home Folder
```html
<td>
    <a asp-action="index" asp-controller="home" asp-route-id="@Model?.ProductId"
        class="btn btn-sm btn-info text-white">
        Select
    </a>
</td>
```
asp-action 和 asp-controller 属性指定操作方法的名称和定义它的控制器。段变量的值是使用 asp-route-[name] 属性定义的，这样 asp-route-id 属性为 id 段变量提供一个值，该变量用于为 asp-action 选择的操作方法提供参数属性。
如果您检查 Select 锚元素，您会看到每个 href 属性都包含与其相关的 Product 对象的 ProductId 值，如下所示：
```html
...
<a class="btn btn-sm btn-info text-white" href="/Home/index/3">Select</a>
...
```
在这种情况下，asp-route-id 属性提供的值意味着无法使用默认 URL，因此路由系统生成了一个 URL，其中包含控制器和操作名称的段，以及将要使用的段为操作方法提供参数。在这两种情况下，由于只指定了一个操作方法，标签助手创建的 URL 以呈现视图的控制器为目标。单击锚点元素将发送一个以 Home 控制器的 Index 方法为目标的 HTTP GET 请求。

### Using Anchor Elements for Razor Pages
asp-page 属性用于将 Razor 页面指定为锚元素的 href 属性的目标。页面路径以 / 字符为前缀，@page 指令定义的路由段的值使用 asp-route-[name] 属性定义。清单 26-8 添加了一个定位元素，该元素以 Pages/Suppliers 文件夹中定义的列表页面为目标。

**GENERATING URLS (AND NOT LINKS)**
标签助手仅在锚元素中生成 URL。如果您需要生成 URL 而不是链接，则可以使用 Url 属性，该属性在控制器、页面模型和视图中可用。该属性返回一个实现 IUrlHelper 接口的对象，该接口提供一组生成 URL 的方法和扩展方法。下面是一个在视图中生成 URL 的 Razor 片段： 
```html
...
<div>@Url.Page("/suppliers/list")</div>
...
```
该片段生成一个 div 元素，其内容是指向 /Suppliers/List Razor Page 的 URL。控制器或页面模型类中使用相同的接口，例如以下语句：
```cs
...
string url = Url.Action("List", "Home");
...
```
该语句生成一个 URL，该 URL 以 Home 控制器上的 List 操作为目标，并将其分配给名为 url 的字符串变量。

## Using the JavaScript and CSS Tag Helpers
ASP.NET Core 提供了标签助手，用于通过脚本和链接元素管理 JavaScript 文件和 CSS 样式表。正如您将在接下来的部分中看到的那样，这些标签助手功能强大且灵活，但需要密切注意以避免产生意外结果。 

### Managing JavaScript Files
ScriptTagHelper 类是脚本元素的内置标记助手，用于使用表 26-4 中描述的属性管理 JavaScript 文件在视图中的包含，我将在接下来的部分中对其进行描述。 
Table 26-4. The Built-in Tag Helper Attributes for script Elements
| Name | Description |
|-|-|
| asp-src-include | 此属性用于指定将包含在视图中的 JavaScript 文件。|
| asp-src-exclude | 此属性用于指定将从视图中排除的 JavaScript 文件。|
| asp-append-version | 该属性用于缓存清除，如“了解缓存清除”边栏中所述。|
| asp-fallback-src | 此属性用于指定在内容分发网络出现问题时使用的回退 JavaScript 文件。|
| asp-fallback-src-include | 此属性用于选择在存在内容分发网络问题时将使用的 JavaScript 文件。|
| asp-fallback-src-exclude | 此属性用于排除 JavaScript 文件以在存在内容分发网络问题时显示它们的用途。|
| asp-fallback-test | 此属性用于指定将用于确定 JavaScript 代码是否已从内容分发网络正确加载的 JavaScript 片段。|

asp-src-include 属性用于使用通配模式将 JavaScript 文件包含在视图中。通配模式支持一组用于匹配文件的通配符，表 26-5 描述了最常见的通配模式。