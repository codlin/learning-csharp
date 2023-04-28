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
