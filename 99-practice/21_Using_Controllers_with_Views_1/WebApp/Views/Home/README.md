# Working with Razor Views

Razor 视图包含 HTML 元素和 C# 表达式。表达式与 HTML 元素混合在一起并用 @ 字符表示，如下所示：
```html
...
<tr><th>Name</th><td>@Model?.Name</td></tr>
...
```
当视图用于生成响应时，将对表达式求值，并将结果包含在发送给客户端的内容中。此表达式获取操作方法提供的 Product 视图模型对象的名称，并生成如下输出：
```html
...
<tr><th>Name</th><td>Corner Flags</td></tr>
...
```
这种转变看起来很神奇，但 Razor 比最初看起来要简单。 Razor 视图被转换为继承自 RazorPage 类的 C# 类，然后像任何其他 C# 类一样进行编译。

## SEEING THE COMPILED OUTPUT FROM RAZOR VIEWS
默认情况下，Razor Views 直接编译成 DLL，生成的 C# 类在构建过程中不会写入磁盘。您可以通过将以下设置添加到 WebApp.csproj 文件来查看生成的类，
在 Visual Studio 中通过右键单击解决方案资源管理器中的 WebApp 项并从弹出菜单中选择编辑项目文件来访问该文件：
```xml
...
<PropertyGroup>
<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
</PropertyGroup>
...
```
保存项目文件并使用 dotnet build 命令构建项目。从 Razor 视图生成的 C# 文件将写入 obj/Debug/net6.0/generated 文件夹。

视图中的表达式被转换为对 Write 方法的调用，该方法对表达式的结果进行编码，以便它可以安全地包含在 HTML 文档中。 
WriteLiteral 方法用于处理视图的静态 HTML 区域，不需要进一步编码。结果是 CSHTML 文件中的这样一个片段：
```HTML
...
<tr><th>Name</th><td>@Model?.Name</td></tr>
...
```
这会在 ExecuteAsync 方法中转换为一系列 C# 语句：
```c#
WriteLiteral("<th>Name</th><td>");
Write(Model?.Name);
WriteLiteral("</td></tr>");
```
调用 ExecuteAsync 方法时，将生成包含静态 HTML 和视图中包含的表达式的响应。计算表达式的结果被写入响应，生成如下 HTML：
```HTML
<th>Name</th><td>Kayak</td></tr>
```

## CACHING RESPONSES
可以通过将 ResponseCache 属性应用于操作方法（或控制器类，它缓存来自所有操作方法的响应）来缓存来自视图的响应。有关如何启用响应缓存的详细信息，请参阅第 17 章。

## Setting the View Model Type
`Watersports.cshtml` 文件的生成类派生自 RazorPage<T>，但 Razor 不知道视图模型的操作方法将使用什么类型，因此它选择动态作为泛型类型参数。
这意味着 @Model 表达式可以与任何属性或方法名称一起使用，这些名称会在运行时生成响应时进行评估。
为了演示使用不存在的成员时会发生什么，请将清单 21-14 中所示的内容添加到 Watersports.cshtml 文件中。
```html
<tr><th>Tax Rate</th><td>@Model?.TaxRate</td></tr>
```
因为不存在`TaxRate`这个字段，所以引发异常。  
**要在开发过程中检查表达式，可以使用 model 关键字指定模型对象的类型。**
```html
@model WebApp.Models.Product
<!DOCTYPE html>
<html>
```

# Understanding Directives
| Name | Description |
|-|-|
| @model | This directive specifies the type of the view model. |
| @using | This directive imports a namespace. |
| @page | This directive denotes a Razor Page, described in Chapter 23. |
| @section | This directive denotes a layout section, as described in Chapter 22. |
| @addTagHelper | This directive adds tag helpers to a view, as described in Chapter 25. |
| @namespace | This directive sets the namespace for the C# class generated from a view. |
| @functions | This directive adds C# properties and methods to the C# class generated from a view and is commonly used in Razor Pages, as described in Chapter 23. |
| @attribute | This directive adds an attribute to the C# class generated from a view. I use this feature to apply authorization restrictions in Chapter 38. |
| @implements | This directive declares that the C# class generated from a view implements an interface. This feature is demonstrated in Chapter 36. |
| @inherits | This directive sets the base class for the C# class generated from a view. This feature is demonstrated in Chapter 36. |
| @inject | This directive provides a view with direct access to a service through dependency injection. This feature is demonstrated in Chapter 23. |

# Understanding Content Expressions
Razor 内容表达式生成的内容包含在视图生成的输出中。表 21-6 描述了最有用的内容表达式，这些表达式将在后面的部分中进行演示。  
**Table 21-6.** *Useful Razor Content Expressions*  
| Name | Description |
|-|-|
| @<expression> | 这是基本的 Razor 表达式，它被计算，它产生的结果被插入到响应中。| 
| @if | 此表达式用于根据表达式的结果选择内容区域。有关示例，请参见“使用条件表达式”部分。| 
| @switch | 此表达式用于根据表达式的结果选择内容区域。有关示例，请参见“使用条件表达式”部分。| 
| @foreach | 此表达式为序列中的每个元素生成相同的内容区域。有关示例，请参见“枚举序列”。 | 
| @{ ... } | 该表达式定义了一个代码块。有关示例，请参阅“使用 Razor 代码块”部分。| 
| @: | 此表达式表示未包含在 HTML 元素中的内容部分。有关示例，请参见“使用条件表达式”部分。| 
| @try | 该表达式用于捕获异常。| 
| @await | 此表达式用于执行异步操作，其结果将插入到响应中。有关示例，请参见第 24 章。 | 

## UNDERSTANDING THE USE OF THE NULL CONDITIONAL OPERATOR IN VIEWS
使用@Model 表达式时，需要空条件运算符（? 运算符）来防止出现空模型值，如下所示：
```html
...
<tr><th>Name</th><td>@Model?.Name</td></tr>
...
```
但是**当 @model 用于定义模型类型时，需要一个不可为 null 的类型**，如下所示：
```html
@model Product
```
实际上，如果您在@model 表达式中指定了可为空的引用类型，编译器将报告警告。这不是它看起来的一致性，它反映了 Razor 在幕后的工作方式。
RazorPage<T> 类（从中派生生成的 C# 视图类）定义了视图表达式中使用的 Model 属性，如表 21-3 中所述。  
RazorPage<T>.Model 属性定义如下：
```c#
...
public T? Model => ViewData == null ? default(T) : ViewData.Model;
...
```
这意味着**即使在 @model 表达式中使用了不可为 null 的 Product 类型，在 @Model 表达式中使用的 Model 属性的类型也是可为 null 的 `Product?` 类型，正是由于这个原因，在视图表达式中需要 null 条件运算符**。

```html
<tr><th>Tax</th><td>@Model?.Price * 0.2m</td></tr>
<tr><th>Tax</th><td>@(Model?.Price * 0.2m)</td></tr>
```
最终生成为：
```html
<tr><th>Tax</th><td>275.00 * 0.2m</td></tr>
<tr><th>Tax</th><td>55.000</td></tr>
```
Razor View 编译器保守地匹配表达式，并假定第一个表达式中的星号和数值是静态内容。第二个表达式的**括号**避免了这个问题。

## Setting Attribute Values
数据属性是名称以 data- 为前缀的属性，多年来一直是创建自定义属性的非正式方式，并作为 HTML5 的一部分成为正式标准的一部分。它们最常用于使 JavaScript 代码可以定位特定元素，或者使 CSS 样式可以更窄地应用。
```html
<table data-id="@Model?.ProductId">
```

## Using Conditional Expressions
Razor 支持条件表达式，这意味着可以根据视图模型定制输出。此技术是 Razor 的核心，允许您从易于阅读和维护的视图创建复杂而流畅的响应。
```cs
@if (Model?.Price > 200) {
    <tr><th>Name</th><td>Luxury @Model?.Name</td></tr>
} else {
    <tr><th>Name</th><td>Basic @Model?.Name</td></tr>
}
```
Razor 还支持 @switch 表达式，这是一种处理多个条件的更简洁的方式，如清单 21-22 所示。
```cs
@switch (Model?.Name) {
case "Kayak":
    <tr><th>Name</th><td>Small Boat</td></tr>
    break;
case "Lifejacket":
    <tr><th>Name</th><td>Flotation Aid</td></tr>
    break;
default:
    <tr><th>Name</th><td>@Model?.Name</td></tr>
    break;
}
```
条件表达式可能导致每个结果子句重复相同的内容块。例如，在 switch 表达式中，每个 case 子句仅在 td 元素的内容上有所不同，而 tr 和 th 元素保持不变。  
要删除这种重复，可以在元素中使用条件表达式。
```cs
<tr><th>Name</th><td>
    @switch (Model?.Name) {
        case "Kayak":
            @:Small Boat
            break;
        case "Lifejacket":
            @:Flotation Aid
            break;
        default:
            @Model?.Name
            break;
    }
</td></tr>
```
Razor 编译器需要有关未包含在 HTML 元素中的文字值的帮助，需要 @: 前缀，如下所示：
```cs
@:Small Boat
```
编译器会处理 HTML 元素，因为它会检测到开放标记，但文本内容需要此额外帮助。
