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