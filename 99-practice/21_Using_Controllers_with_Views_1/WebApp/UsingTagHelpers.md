# 25. Using Tag Helpers

标签助手`Tag Helpers`是 C# 类，可以转换视图或页面中的 HTML 元素。标签助手的常见用途包括使用应用程序的路由配置为表单生成 URL，确保特定类型的元素样式一致，以及用常用的内容片段替换自定义速记元素。在本章中，我将描述标签助手的工作原理以及自定义标签助手的创建和应用方式。在第 26 章中，我描述了内置的标签助手，在第 27 章中，我使用标签助手来解释如何创建 HTML 表单。表 25-1 将标签助手放在上下文中。
Table 25-1. Putting Tag Helpers in Context
| Question | Answer |
|-|-|
| 标签助手是什么？| 标签助手是操纵 HTML 元素的类，以某种方式更改它们，用额外的内容补充它们，或者用新内容完全替换它们。 |
| 为什么它们有用？| 标签助手允许使用 C# 逻辑生成或转换视图内容，确保发送到客户端的 HTML 反映应用程序的状态。|
| 如何使用它们？  | 应用标签助手的 HTML 元素是根据类的名称或 HTMLTargetElement 属性来选择的。呈现视图时，元素由标签助手转换并包含在发送给客户端的 HTML 中。|
| 有什么坑？     | 使用标签助手可以很容易得意忘形并生成复杂的 HTML 内容部分，这使用视图组件更容易实现，如第 24 章所述。 |
| 有什么替代品？  | 您不必使用标签助手，但它们可以轻松地在 ASP.NET Core 应用程序中生成复杂的 HTML。|

## Creating a Tag Helper
理解标签助手的最佳方式是创建一个标签助手，它揭示了它们的操作方式以及它们如何适应 ASP.NET Core 应用程序。在接下来的部分中，我将完成创建和应用标签助手的过程，该标签助手将为 tr 元素设置 Bootstrap CSS 类，以便像这样的元素：
```html
...
<tr tr-color="primary">
    <th colspan="2">Product Summary</th>
</tr>
...
```
会被转换为：
```html
...
<tr class="bg-primary text-white text-center">
    <th colspan="2">Product Summary</th>
</tr>
...
```
标签助手将识别 tr-color 属性并使用它的值来设置发送到浏览器的元素的类属性。这不是最引人注目或最有用的转换，但它为解释标签助手的工作原理提供了基础。

### Defining the Tag Helper Class
标签助手可以在项目的任何地方定义，但将它们放在一起会有所帮助，因为它们需要在使用前注册。创建 WebApp/TagHelpers 文件夹并向其中添加一个名为 TrTagHelper.cs 的类文件，代码如清单 25-6 所示。
标签助手派生自 TagHelper 类，该类在 Microsoft.AspNetCore.Razor.TagHelpers 命名空间中定义。 TagHelper 类定义了一个 Process 方法，该方法被子类覆盖以实现转换元素的行为。标签助手的名称结合了它转换的元素的名称，后跟 TagHelper。在这个例子中，类名 TrTagHelper 表明这是一个对 tr 元素进行操作的标签助手。可以使用属性来扩大或缩小可以应用标签助手的元素范围，如本章后面所述，但默认行为由类名定义。
可以通过重写 ProcessAsync 方法而不是 Process 方法来创建异步标记帮助器，但这对于大多数帮助器来说不是必需的，它们往往会对 HTML 元素进行小而集中的更改。您可以在“高级标记帮助程序功能”部分中查看异步标记帮助程序的示例。

**Receiving Context Data**
标签助手通过 TagHelperContext 类的实例接收有关它们正在转换的元素的信息，该实例作为 Process 方法的参数接收，并定义表 25-3 中描述的属性。
Table 25-3. The TagHelperContext Properties
| Name | Description |
|-|-|
| AllAttributes | 此属性返回应用于正在转换的元素的属性的只读字典，按名称和索引索引。|
| Items | 此属性返回一个字典，用于在标签助手之间进行协调，如“标签助手之间的协调”部分所述。|
| UniqueId | 此属性返回正在转换的元素的唯一标识符。 |
虽然您可以通过 AllAttributes 字典访问元素属性的详细信息，但更方便的方法是定义一个属性，其名称对应于您感兴趣的属性，如下所示：
```cs
...
public string BgColor { get; set; } = "dark";
public string TextColor { get; set; } = "white";
...
```
When a tag helper is being used, the properties it defines are inspected and assigned the value of any
whose name matches attributes applied to the HTML element.
当使用标签助手时，将检查它定义的属性，并为其分配名称与应用于HTML元素的属性相匹配的任何属性的值。  
作为`Process`的一部分，属性值将被转换以匹配 C# 属性的类型，以便 bool 属性可用于接收 true 和 false 属性值，因此 int 属性可用于接收数字属性值，例如 1 和2. 对应的 HTML 元素属性中没有的 C# 类属性不会被设置，这意味着您应该检查以确保您没有处理 null 或提供默认值。    
属性的名称会自动从默认的 HTML 样式 bg-color 转换为 C# 样式 BgColor。您可以使用除 asp-（Microsoft 使用）和 data-（为发送到客户端的自定义属性保留）之外的任何属性前缀。示例标签助手将使用 bg-color 和 text-color 属性进行配置，这将为 BgColor 和 TextColor 属性提供值，并用于在 Process 方法中配置 tr 元素，如下所示：
```cs
...
output.Attributes.SetAttribute("class", $"bg-{BgColor} text-center text-{TextColor}");
...
```
将 HTML 属性名称用于标签助手属性并不总是会产生可读或可理解的类。您可以使用 HtmlAttributeName 属性断开属性名称和它表示的属性之间的链接，该属性可用于指定属性表示的 HTML 属性。  

**Producing Output**
Process 方法通过配置作为参数接收的 TagHelperOutput 对象来转换元素。 TagHelperOuput 对象首先描述出现在视图中的 HTML 元素，然后通过表 25-4 中描述的属性和方法进行修改。
Table 25-4. The TagHelperOutput Properties and Methods
*忽略*

在 TrTagHelper 类中，我使用 Attributes 字典向指定 Bootstrap 样式的 HTML 元素添加了一个类属性，包括 BgColor 和 TextColor 属性的值。效果是可以通过将 bg-color 和 text-color 属性设置为 Bootstrap 名称（例如 primary、info 和 danger）来指定 tr 元素的背景色。

### Registering Tag Helpers
**标签助手类必须在使用前使用 @addTagHelper 指令注册**。可以应用标签助手的视图或页面集取决于 @addTagHelper 指令的使用位置。对于单个视图或页面，该指令出现在 CSHTML 文件本身中。**为了使标记帮助程序更广泛地可用，可以将其添加到视图导入文件中，该文件在控制器的 Views 文件夹和 Razor Pages 的 Pages 文件夹中定义**。  
我希望我在本章中创建的标签助手在应用程序的任何地方都可用，这意味着 @addTagHelper 指令被添加到 Views 和 Pages 文件夹中的 _ViewImports.cshtml 文件中。第 24 章中用于应用视图组件的 vc 元素是一个标签助手，这就是为什么启用标签助手所需的指令已经在 _ViewImports.cshtml 文件中。
```cs
@addTagHelper *, WebApp
```
**参数的第一部分指定标签助手类的名称，支持通配符，第二部分指定定义它们的程序集的名称**。此 @addTagHelper 指令使用通配符来选择 WebApp 程序集中的所有命名空间，其效果是可以在任何控制器视图中使用项目中任何位置定义的标记助手。 Pages 文件夹中的 Razor Pages _ViewImports.cshtml 文件中有相同的语句。  
另一个 @addTagHelper 指令启用 Microsoft 提供的内置标签助手，这在第 26 章中有描述。
```cs
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

### Using a Tag Helper
最后一步是使用标签助手来转换元素。在清单 25-7 中，我将属性添加到 tr 元素，它将应用标签助手。
Listing 25-7. Using a Tag Helper in the Index.cshtml File in the Views/Home Folder
```html
<tr bg-color="info" text-color="white"></tr>
```
示例 25-7 中应用了属性的 tr 元素已经被转换，但这并不是图中显示的唯一变化。**默认情况下，标签助手适用于特定类型的所有元素，这意味着视图中的所有 tr 元素都已使用标签助手类中定义的默认值进行转换，因为没有定义任何属性**。 （有些表行没有显示文本的原因是因为 Bootstrap table-striped 类，它对交替行应用不同的样式。）  
实际上，问题更严重，因为视图导入文件中的 @addTagHelper 指令意味着example 标签助手应用于控制器和 Razor Pages 呈现的任何视图中使用的所有 tr 元素。例如，使用浏览器请求 http://localhost:5000/cities，您将看到来自 Cities Razor Page 的响应中的 tr 元素也发生了转换，如图 25-3 所示。

### Narrowing the Scope of a Tag Helper
可以使用 HtmlTargetElement 元素控制由标签助手转换的元素范围，如清单 25-8 所示。
Listing 25-8. Narrowing Scope in the TrTagHelper.cs File in the TagHelpers Folder
```cs
[HtmlTargetElement("tr", Attributes = "bg-color,text-color", ParentTag = "thead")]
public class TrTagHelper : TagHelper { ... }
```
Table 25-5. The HtmlTargetElement Properties
| Name | Description |
|-|-|
| Attributes | 此属性用于指定标签助手应仅应用于具有给定属性集的元素，以逗号分隔的列表形式提供。以星号结尾的属性名称将被视为前缀，因此 bg-* 将匹配 bg-color、bgsize 等。|
| ParentTag | 此属性用于指定标签助手应仅应用于给定类型的元素中包含的元素。|
| TagStructure | 此属性用于指定标签助手应仅应用于其标签结构对应于 TagStructure 枚举中给定值的元素，该枚举定义了 Unspecified、NormalOrSelfClosing 和 WithoutEndTag。|

### Widening the Scope of a Tag Helper
HtmlTargetElement 属性也可用于扩大标签助手的范围，使其匹配更广泛的元素。这是通过将属性的第一个参数设置为星号（* 字符）来完成的，它可以匹配任何元素。清单 25-9 更改了应用于示例标签助手的属性，以便它匹配任何具有 bg-color 和 text-color 属性的元素。
```cs
[HtmlTargetElement("*", Attributes = "bg-color,text-color")]
public class TrTagHelper : TagHelper { ... }
```
平衡 TagHelpers 文件夹中 TrTagHelper.cs 文件中的范围
```cs
[HtmlTargetElement("tr", Attributes = "bg-color,text-color")]
[HtmlTargetElement("td", Attributes = "bg-color")]
public class TrTagHelper : TagHelper { ... }
```
如果需要对一个元素应用多个标签助手，可以通过设置从 TagHelper 基类继承的 Order 属性来控制它们执行的顺序。管理序列可以帮助最小化标签助手之间的冲突，尽管仍然很容易遇到问题。

## Advanced Tag Helper Features
### Creating Shorthand Elements
标签助手不仅限于转换标准 HTML 元素，还可以用于用常用内容替换自定义元素。这可能是一个有用的特性，可以使视图更简洁并使它们的意图更明显。为了演示，清单 25-12 用自定义 HTML 元素替换了 Index 视图中的 thead 元素。
清单 25-12在 Views/Home 文件夹中的 Index.cshtml 文件中添加自定义 HTML 元素
```html
<tablehead bg-color="dark">Product Summary</tablehead>
```
tablehead 元素不是 HTML 规范的一部分，浏览器无法理解。相反，我将使用此元素作为生成 thead 元素及其 HTML 表格内容的简写。  
将名为 TableHeadTagHelper.cs 的类添加到 TagHelpers 文件夹，并使用它来定义如清单 25-13 所示的类。
提示： 在处理不属于 HTML 规范的自定义元素时，您必须应用 HtmlTargetElement 属性并指定元素名称，如清单 25-13 所示。基于类名将标签助手应用于元素的约定仅适用于标准元素名称。
Listing 25-13. The Contents of TableHeadTagHelper.cs in the TagHelpers Folder
**见文件**
此标签助手是异步的并重写 ProcessAsync 方法，以便它可以访问它转换的元素的现有内容。 ProcessAsync方法使用TagHelperOuput对象的属性生成一个完全不同的元素：TagName属性用于指定一个thead元素，TagMode属性用于指定该元素使用开始和结束标签编写，Attributes.SetAttribute方法用于定义类属性，Content属性用于设置元素内容。元素的现有内容是通过异步 GetChildContentAsync 方法获取的，该方法返回一个 TagHelperContent 对象。这是由 TagHelperOutput.Content 属性返回的同一对象，并允许使用相同类型通过表 25-6 中描述的方法检查和更改元素的内容。
Table 25-6. Useful TagHelperContent Methods
**略**
